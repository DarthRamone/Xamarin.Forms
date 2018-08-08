using System;
using SkiaSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Linq;
namespace Xamarin.Forms.Platform.Skia
{
	public static class ImageCache
	{
		static object locker = new object();
		static Dictionary<string, SKBitmap> bitmaps = new Dictionary<string, SKBitmap>();
		static Dictionary<string, TaskCompletionSource<bool>> tasks = new Dictionary<string, TaskCompletionSource<bool>>();
		static Dictionary<string, string> urlRequests = new Dictionary<string, string>();
		static SimpleQueue<string> urlQueue = new SimpleQueue<string>();

		public static SKBitmap TryGetValue(string url)
		{
			lock(locker)
			{
				bitmaps.TryGetValue(url, out var bitmap);
				return bitmap;
			}
		}

		public static Task LoadImage(string url, string requestId)
		{
			lock (locker)
			{
				if(!tasks.TryGetValue(url, out var task)){
					var tcs = new TaskCompletionSource<bool>();
					tasks[url] =  tcs;
					urlRequests[url] = requestId;
					urlQueue.Enqueue(url);
					RunDownloader();
				}
				return task.Task;
			}
		}

		static Task downloadTask;
		static Task RunDownloader()
		{
			lock(locker)
			{
				if (downloadTask?.IsCompleted ?? true)
					downloadTask = _runDownloader();
				return downloadTask;
			}
		}
		static async Task _runDownloader()
		{
			while(urlQueue.Count > 0)
			{
				await downloadFirstUrl(); 
			}
		}

		static HttpClient httpClient = new HttpClient();
		static async Task downloadFirstUrl()
		{
			bool success = false;
			var url = urlQueue.Dequeue();
			try
			{
				using (Stream stream = await httpClient.GetStreamAsync(url))
				using (MemoryStream memStream = new MemoryStream())
				{
					await stream.CopyToAsync(memStream);
					memStream.Seek(0, SeekOrigin.Begin);
					var webBitmap = SKBitmap.Decode(stream);
					lock (locker)
					{
						bitmaps[url] = webBitmap;
					}
					success = true;
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				success = false;
			}
			finally
			{
				lock (locker)
				{
					var task = tasks[url];
					task.TrySetResult(success);
				}
			}
		}

		public static void ClearCache(string requestId)
		{
			lock (locker)
			{
				var urls = urlRequests.Where(x => x.Value == requestId).Select(x=> x.Key).ToList();
				foreach(var url in urls)
				{
					bitmaps.Remove(url);
					var task = tasks[url];
					task.TrySetCanceled();
					tasks.Remove(url);
					urlQueue.Remove(url);
					urlRequests.Remove(url);
				}
			}

		}
	}
}

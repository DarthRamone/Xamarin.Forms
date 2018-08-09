using System;
using SkiaSharp;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
namespace Xamarin.Forms.Platform.Skia
{
	public static class ImageCache
	{
		static ImageCache()
		{
			Directory.CreateDirectory(CacheFolder);
		}
		static string BaseCacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Previewer");
		static string CacheFolder = Path.Combine(BaseCacheDirectory, "Cache");
		static object locker = new object();
		static Dictionary<string, SKBitmap> bitmaps = new Dictionary<string, SKBitmap>();
		static Dictionary<string, TaskCompletionSource<bool>> tasks = new Dictionary<string, TaskCompletionSource<bool>>();
		static Dictionary<string, string> urlRequests = new Dictionary<string, string>();
		static SimpleQueue<string> urlQueue = new SimpleQueue<string>();

		public static SKBitmap TryGetValue(string url)
		{
			lock (locker)
			{
				bitmaps.TryGetValue(url, out var bitmap);
				if(bitmap == null)
				{
					bitmap = LoadFromLocalCache(url);
					if (bitmap != null)
						bitmaps[url] = bitmap;
				}
				return bitmap;
			}
		}

		static SKBitmap LoadFromLocalCache(string url)
		{
			var caceFile = CacheFile(url);
			if (File.Exists(caceFile))
				return SKBitmap.Decode(caceFile);
			return null;
		}
		static string CacheFile(string url) => Path.Combine(CacheFolder, MD5Hash(url));

		public static Task<bool> LoadImage(string url, string requestId)
		{
			lock (locker)
			{
				if (!tasks.TryGetValue(url, out var task))
				{
					var tcs = new TaskCompletionSource<bool>();
					task = tasks[url] = tcs;
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
			lock (locker)
			{
				if (downloadTask?.IsCompleted ?? true)
					downloadTask = Task.Run(_runDownloader);
				return downloadTask;
			}
		}
		static async Task _runDownloader()
		{
			while (urlQueue.Count > 0)
			{
				downloadFirstUrl();
			}
		}

		static HttpClient httpClient = new HttpClient();
		static async Task downloadFirstUrl()
		{
			bool success = false;
			var url = urlQueue.Dequeue();
			var cache = CacheFile(url);
			try
			{
				using (Stream stream = await httpClient.GetStreamAsync(url))
				using (var fileStream = new FileStream(cache, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					await stream.CopyToAsync(fileStream);
					var webBitmap = SKBitmap.Decode(cache);
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
				if (File.Exists(cache))
					File.Delete(cache);
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
				var urls = urlRequests.Where(x => x.Value == requestId).Select(x => x.Key).ToList();
				foreach (var url in urls)
				{
					bitmaps.Remove(url);
					var task = tasks[url];
					task.TrySetResult(false);
					tasks.Remove(url);
					urlQueue.Remove(url);
					urlRequests.Remove(url);
				}
			}

		}
		public static string MD5Hash(string input)
		{
			var hash = new StringBuilder();
			var md5provider = new MD5CryptoServiceProvider();
			var bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));
			for (int i = 0; i < bytes.Length; i++)
			{
				hash.Append(bytes[i].ToString("x2"));
			}
			return hash.ToString();
		}
	}
}

using SkiaSharp;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Skia
{
	public class Platform : IPlatform
	{
		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			SizeRequest? result = null;

			if (view is Button button)
			{
				var text = button.Text;

				var paint = new SKPaint
				{
					TextSize = (float)button.FontSize,
					IsAntialias = true
				};

				var bounds = new SKRect();
				paint.MeasureText(text, ref bounds);

				bounds.Inflate(10, 10);
				return new SizeRequest(new Size(bounds.Width, bounds.Height));
			}
			else if (view is Label label)
			{
				var text = label.Text;

				var paint = new SKPaint
				{
					TextSize = (float)label.FontSize,
					IsAntialias = true
				};

				var bounds = new SKRect();
				paint.MeasureText(text, ref bounds);

				return new SizeRequest(new Size(bounds.Width, bounds.Height));
			}
			else if (view is Image image)
			{
				return new SizeRequest(new Size(100, 100));
			}

			if (result == null)
				throw new NotImplementedException();

			return result.Value;
		}
	}
}

using SkiaSharp;
using System;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Skia
{
	public class Platform : IPlatform
	{
		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			SizeRequest? result = null;

			if (view is Button || view is Label)
			{
				string text = null;
				float textSize = 0;
				if (view is Button button)
				{
					text = button.Text;
					textSize = (float)button.FontSize;
				}
				else if (view is Label label)
				{
					text = label.Text;
					textSize = (float)label.FontSize;
				}

				var paint = new SKPaint
				{
					TextSize = textSize,
					IsAntialias = true
				};

				var bounds = new SKRect();
				paint.MeasureText(text, ref bounds);

				var size = new Size(bounds.Width, paint.TextSize * 1.25);

				if (view is Button)
					size += new Size(10, 10);

				return new SizeRequest(size);
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

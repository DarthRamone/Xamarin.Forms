using SkiaSharp;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.Skia
{

	public static class Forms
	{
		public static IPlatform Platform = new Platform();

		public static void Init ()
		{
			Device.PlatformServices = new SkiaPlatformServices();
			Device.Info = new SkiaDeviceInfo();
		}

		public static void Draw (Element element, Rectangle region, SKSurface surface)
		{
			var canvas = surface.Canvas;

			canvas.Clear(SKColors.White);

			element.Platform = Platform;
			foreach (var e in element.Descendants())
				if (e is VisualElement v)
					v.IsPlatformEnabled = true;
			if (element is VisualElement ve)
			{
				ve.IsPlatformEnabled = true;
				ve.Layout(region);
			}

			Stack<Element> drawStack = new Stack<Element>();
			drawStack.Push(element);

			while(drawStack.Count > 0)
			{
				var current = drawStack.Pop();

				foreach (var child in current.LogicalChildren)
				{
					drawStack.Push(child);
				}

				DrawElement(current, canvas);
			}
		}

		private static void DrawElement(Element element, SKCanvas canvas)
		{
			if (element is ContentPage page)
			{
				DrawContentPage(page, canvas);
			}
			else if (element is Label label)
			{
				DrawLabel(label, canvas);
			}
		}

		private static void DrawContentPage(ContentPage page, SKCanvas canvas)
		{
			canvas.Clear(page.BackgroundColor.ToSKColor());
		}

		private static void DrawLabel(Label label, SKCanvas canvas)
		{
			var paint = new SKPaint();
			paint.Color = label.TextColor.ToSKColor();
			paint.IsAntialias = true;
			paint.TextSize = (float)label.FontSize;

			canvas.DrawText(label.Text, new SKPoint((float)label.X, (float)label.Y), paint);
		}
	}
}

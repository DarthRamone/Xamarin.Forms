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
			else if (element is Button button)
			{
				DrawButton(button, canvas);
			}
		}

		private static void DrawVisualElement(VisualElement ve, SKCanvas canvas)
		{
			var paint = new SKPaint();
			paint.Color = ve.BackgroundColor.ToSKColor(Color.Transparent);
			canvas.DrawRect(ve.Bounds.ToSKRect(), paint);
		}

		private static void DrawContentPage(ContentPage page, SKCanvas canvas)
		{
			DrawVisualElement(page, canvas);
		}

		private static void DrawButton(Button button, SKCanvas canvas)
		{
			//-----------------------------------------------------------------------------
			// Draw Group shape group
			// Shadow color for RoundRectangleStyleFill
			var RoundRectangleStyleFillShadowColor = new SKColor(0, 0, 0, 20);

			// Build shadow for RoundRectangleStyleFill
			var RoundRectangleStyleFillShadow = SKImageFilter.CreateDropShadow(0, 0, 4, 4, RoundRectangleStyleFillShadowColor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground, null, null);

			// Fill color for Round Rectangle Style
			var RoundRectangleStyleFillColor = button.BackgroundColor.ToSKColor(new Color(0.7));

			// New Round Rectangle Style fill paint
			var RoundRectangleStyleFillPaint = new SKPaint()
			{
				Style = SKPaintStyle.Fill,
				Color = RoundRectangleStyleFillColor,
				BlendMode = SKBlendMode.SrcOver,
				IsAntialias = true,
				ImageFilter = RoundRectangleStyleFillShadow
			};

			// Frame color for Round Rectangle Style
			var RoundRectangleStyleFrameColor = button.BorderColor.ToSKColor();

			// New Round Rectangle Style frame paint
			var RoundRectangleStyleFramePaint = new SKPaint()
			{
				Style = SKPaintStyle.Stroke,
				Color = RoundRectangleStyleFrameColor,
				BlendMode = SKBlendMode.SrcOver,
				IsAntialias = true,
				StrokeWidth = (float)button.BorderWidth,
			};

			float rounding = (float)button.CornerRadius;
			if (rounding < 0)
				rounding = 0;

			// Draw Round Rectangle shape
			var bounds = button.Bounds.Inflate(-4, -4);

			canvas.DrawRoundRect(bounds.ToSKRect(), rounding, rounding, RoundRectangleStyleFillPaint);
			canvas.DrawRoundRect(bounds.ToSKRect(), rounding, rounding, RoundRectangleStyleFramePaint);

			DrawText(button.Text, canvas, button.TextColor, button.FontSize, button.X, button.Y);
		}

		private static void DrawText(string text, SKCanvas canvas, Color textColor, double fontSize, double x, double y)
		{
			var paint = new SKPaint
			{
				Color = textColor.ToSKColor(),
				IsAntialias = true,
				TextSize = (float)fontSize,
			};
			if(!string.IsNullOrWhiteSpace(text))
				canvas.DrawText(text, (float)x, (float)y + paint.TextSize, paint);
		}

		private static void DrawLabel(Label label, SKCanvas canvas)
		{
			DrawVisualElement(label, canvas);

			DrawText(label.Text, canvas, label.TextColor, label.FontSize, label.X, label.Y);
		}
	}
}

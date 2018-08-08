using SkiaSharp;
using System;
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
			var RoundRectangleStyleFillColor = button.BackgroundColor.ToSKColor(Color.Transparent);

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

			DrawText(label.Text, canvas, new TextDrawingData()
			{
				Color = label.TextColor,
				Rect = label.Bounds,
				FontSize = label.FontSize,
				Wrapping = label.LineBreakMode,
			});
		}

		public class LineInfo
		{
			public string Text { get; set; }
			public float Width { get; set; }
			public SKPoint Origin { get; set; }
			public float Height { get; set; }

			public LineInfo(string text, float width, float height, SKPoint origin)
			{
				Text = text;
				Width = width;
				Origin = origin;
				Height = height;
			}
		}

		public static void GetTextLayout (string text, TextDrawingData data, out List<LineInfo> lines)
		{
			var maxWidth = data.Rect.Width;

			var lineHeight = (float)data.FontSize * 1.25f;

			var paint = new SKPaint
			{
				Color = data.Color.ToSKColor(Color.Black),
				IsAntialias = true,
				TextSize = (float)data.FontSize
			};

			lines = new List<LineInfo>();

			var remaining = text;
			float y = paint.TextSize + (float)data.Rect.Top;
			float x = (float)data.Rect.Left;
			while (!string.IsNullOrEmpty(remaining))
			{
				paint.BreakText(remaining, (float)maxWidth, out var measuredWidth, out var measuredText);

				if (measuredText.Length == 0)
					break;

				if (data.Wrapping == LineBreakMode.NoWrap)
				{
					lines.Add(new LineInfo(measuredText, measuredWidth, lineHeight, new SKPoint(x, y)));
					break;
				}
				else if (data.Wrapping == LineBreakMode.WordWrap)
				{
					if (measuredText.Length != remaining.Length)
					{
						for (int i = measuredText.Length - 1; i >= 0; i--)
						{
							if (char.IsWhiteSpace(measuredText[i]))
							{
								measuredText = measuredText.Substring(0, i + 1);
								break;
							}
						}
					}

					lines.Add(new LineInfo(measuredText, paint.MeasureText(measuredText), lineHeight, new SKPoint(x, y)));
				}
				else if (data.Wrapping == LineBreakMode.HeadTruncation)
				{
					throw new NotImplementedException();
				}
				else if (data.Wrapping == LineBreakMode.TailTruncation)
				{
					throw new NotImplementedException();
				}
				else if (data.Wrapping == LineBreakMode.MiddleTruncation)
				{
					throw new NotImplementedException();
				}

				remaining = remaining.Substring(measuredText.Length);

				y += lineHeight;
			}
		}

		private static void DrawText(string text, SKCanvas canvas, TextDrawingData data)
		{
			canvas.Save();

			var paint = new SKPaint
			{
				Color = data.Color.ToSKColor(Color.Black),
				IsAntialias = true,
				TextSize = (float)data.FontSize
			};

			canvas.ClipRect(data.Rect.ToSKRect());

			GetTextLayout(text, data, out var lines);

			foreach (var line in lines)
			{
				if (!string.IsNullOrWhiteSpace(line.Text))
				{
					canvas.DrawText(line.Text, line.Origin, paint);
				}
			}

			canvas.Restore();
		}
	}

	public class TextDrawingData
	{
		public Color Color { get; set; }
		public TextAlignment HAlign { get; set; }
		public TextAlignment VAlign { get; set; }
		public double FontSize { get; set; }
		public Rectangle Rect { get; set; }
		public string FontFamily { get; set; }
		public FontAttributes Attributes { get; set; }
		public LineBreakMode Wrapping { get; set; }
	}
}

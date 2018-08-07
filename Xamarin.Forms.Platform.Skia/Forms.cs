using SkiaSharp;

namespace Xamarin.Forms.Platform.Skia
{

	public static class Forms
	{
		public static void Init ()
		{
			Device.PlatformServices = new SkiaPlatformServices();
			Device.Info = new SkiaDeviceInfo();
		}

		public static void Draw (Element element, Rectangle region, SKSurface surface)
		{
			var canvas = surface.Canvas;

			canvas.Clear(SKColors.Purple);
		}
	}
}

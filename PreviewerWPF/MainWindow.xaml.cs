using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xamarin.Forms.Platform.Skia;

namespace PreviewerWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			Forms.Init();

			InitializeComponent();
		}

		private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			// the the canvas and properties
			var canvas = e.Surface.Canvas;

			// get the screen density for scaling
			var scale = (float)PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
			var scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

			// handle the device screen density
			canvas.Scale(scale);

			//// make sure the canvas is blank
			//canvas.Clear(SKColors.White);

			//// draw some text
			//var paint = new SKPaint
			//{
			//	Color = SKColors.Black,
			//	IsAntialias = true,
			//	Style = SKPaintStyle.Fill,
			//	TextAlign = SKTextAlign.Center,
			//	TextSize = 24
			//};
			//var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
			//canvas.DrawText("SkiaSharp", coord, paint);

			Forms.Draw(null, Xamarin.Forms.Rectangle.Zero, e.Surface);
		}
	}
}

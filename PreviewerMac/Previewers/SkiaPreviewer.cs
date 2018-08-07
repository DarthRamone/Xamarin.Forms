using System;
using System.Threading.Tasks;
using SkiaSharp.Views.Mac;
using Xamarin.Forms;
using Xamarin.Forms.Previewer;

namespace PreviewerMac.Previewers
{
	public class SkiaPreviewer : SKCanvasView, IPreviewer
	{
		public event EventHandler Redraw;

		public Task Draw(Element element, int width, int height)
		{
			throw new NotImplementedException();
		}
	}
}

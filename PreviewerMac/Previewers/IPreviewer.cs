using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PreviewerMac.Previewers
{
	public interface IPreviewer
	{
		event EventHandler Redraw;
		Task Draw(Element element, int width, int height);
	}
}

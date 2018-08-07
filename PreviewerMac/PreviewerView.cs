using System;
using System.ComponentModel;
using AppKit;
using Foundation;
using PreviewerMac.Previewers;
using Xamarin.Forms.Platform.Skia;
using Xamarin.Forms.Previewer;
namespace PreviewerMac
{

	[Register(nameof(PreviewerView))]
	[DesignTimeVisible(true)]
	public class PreviewerView : NSView
	{
		NSComboBox rendererPicker;
		NSComboBox sizePicker;
		NSTextField xamlEntry;
		IPreviewer previewer;
		NSView nativePreviewView;


		public PreviewerView()
		{
			Initialize();
		}

		// created via designer
		public PreviewerView(IntPtr p)
			: base(p)
		{
		}
		// created via designer
		public override void AwakeFromNib()
		{
			Initialize();
		}

		void Initialize()
		{

			rendererPicker = new NSComboBox();
			sizePicker = new NSComboBox();
			xamlEntry = new NSTextField();
			xamlEntry.Changed += XamlEntry_Changed;
			previewer = new SkiaPreviewer();
			xamlEntry.StringValue = XamlParser.XamlSimpleString;
			nativePreviewView = previewer as NSView;

		}

		void XamlEntry_Changed(object sender, EventArgs e)
		{
			var element = XamlParser.ParseXaml(xamlEntry.StringValue);
			//TODO: Get current size
			previewer.Draw(element, 480, 620);
		}


		public override bool IsFlipped => true;
		public override void Layout()
		{
			base.Layout();
			nfloat padding = 6;
			nfloat doublePadding = padding * 2;
			rendererPicker.SizeToFit();
			sizePicker.SizeToFit();

			var topHeight = NMath.Max(rendererPicker.Frame.Height, sizePicker.Frame.Height) + doublePadding;
			var half = Bounds.Width / 2;
			var frame = rendererPicker.Frame;
			frame.Y = frame.X = padding;
			rendererPicker.Frame = frame;

			frame = sizePicker.Frame;
			frame.X = half + padding;
			frame.Y = padding;
			sizePicker.Frame = frame;

			var top = topHeight;
			var sideHeight = this.Bounds.Height - top;
			frame = new CoreGraphics.CGRect(padding, top, half - doublePadding, sideHeight);
			xamlEntry.Frame = frame;

			frame.X = half + padding;
			nativePreviewView.Frame = frame;
		}
	}
}

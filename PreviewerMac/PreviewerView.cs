using System;
using System.ComponentModel;
using AppKit;
using CoreGraphics;
using Foundation;
using PreviewerMac.Previewers;
using Xamarin.Forms.Platform.Skia;
using Xamarin.Forms.Previewer;
using System.Threading.Tasks;
using System.Linq;
namespace PreviewerMac
{

	[Register(nameof(PreviewerView))]
	[DesignTimeVisible(true)]
	public class PreviewerView : NSView
	{
		NSPopUpButton rendererPicker;
		NSPopUpButton sizePicker;
		NSTextView xamlEntry;
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
			//Layer.BackgroundColor = NSColor.Blue.CGColor;
			AddSubview(rendererPicker = new NSPopUpButton());
			rendererPicker.AddItem("Skia");
			AddSubview(sizePicker = new NSPopUpButton());
			ScreenSize.Sizes.ToList().ForEach(x => sizePicker.AddItem(x.Description));
			sizePicker.Activated += (object sender, EventArgs e) => Refresh();
			AddSubview(xamlEntry = new NSTextView
			{
				BackgroundColor = NSColor.White,
				TextColor = NSColor.Black,
				AutomaticQuoteSubstitutionEnabled=false,
			});

			xamlEntry.TextDidChange += XamlEntry_Changed;
			previewer = new SkiaPreviewer();
			xamlEntry.TextStorage.SetString (new NSAttributedString(XamlParser.XamlSimpleString));
			AddSubview(nativePreviewView = previewer as NSView);
			Refresh();

		}

		void XamlEntry_Changed(object sender, EventArgs e)
		{
			Refresh();
		}
		void Refresh()
		{
			var xaml = xamlEntry.TextStorage.Value;
			var (element, error) = String.IsNullOrEmpty(xaml) ? (null,null) : XamlParser.ParseXaml(xaml);
			var size = ScreenSize.Sizes[sizePicker.IndexOfSelectedItem];
			previewer.Draw(element, size.Width, size.Height);
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
			frame.Width = NMath.Max(frame.Width, 200);
			rendererPicker.Frame = frame;

			frame = sizePicker.Frame;
			frame.X = half + padding;
			frame.Y = padding;
			frame.Width = NMath.Max(frame.Width, 200);
			sizePicker.Frame = frame;

			var top = topHeight;
			var sideHeight = this.Bounds.Height - top;
			frame = new CGRect(padding, top, half - doublePadding, sideHeight);
			xamlEntry.Frame = frame;

			frame.X = half + padding;
			nativePreviewView.Frame = frame;
		}

		class SizeSource : NSComboBoxDataSource
		{
			public override nint ItemCount(NSComboBox comboBox) => ScreenSize.Sizes.Length;
			public override NSObject ObjectValueForItem(NSComboBox comboBox, nint index) => (NSString)ScreenSize.Sizes[index].Description;
			public override nint IndexOfItem(NSComboBox comboBox, string value) => Array.IndexOf(ScreenSize.Sizes, ScreenSize.Sizes.FirstOrDefault(x => x.Description == value));
		 
		}
	}
}

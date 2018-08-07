using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace PreviewerMac
{
	public partial class ViewController : NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}
		public override void LoadView()
		{
			View = new PreviewerView();
		}
		public override void ViewWillTransition(CGSize newSize)
		{
			base.ViewWillTransition(newSize);
			// View.Frame = new CGRect(new CGPoint(0, 0), newSize);
		}

		public override NSObject RepresentedObject
		{
			get
			{
				return base.RepresentedObject;
			}
			set
			{
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
	}
}

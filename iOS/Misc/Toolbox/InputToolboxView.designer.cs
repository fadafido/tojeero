// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ObjectX.Touch.Toolbox
{
	partial class InputToolboxView
	{
		[Outlet]
		UIKit.UIButton doneButton { get; set; }

		[Outlet]
		UIKit.UISegmentedControl segmentControl { get; set; }

		[Action ("doneButtonTapped:")]
		partial void doneButtonTapped (Foundation.NSObject sender);

		[Action ("segmentButtonValueChanged:")]
		partial void segmentButtonValueChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (doneButton != null) {
				doneButton.Dispose ();
				doneButton = null;
			}

			if (segmentControl != null) {
				segmentControl.Dispose ();
				segmentControl = null;
			}
		}
	}

	partial class InputToolboxViewOwner
	{
		[Outlet]
		internal ObjectX.Touch.Toolbox.InputToolboxView inputAccessoryView { get; private set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (inputAccessoryView != null) {
				inputAccessoryView.Dispose ();
				inputAccessoryView = null;
			}
		}
	}
}

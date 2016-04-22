using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(Tojeero.Forms.Controls.Picker), typeof(Tojeero.iOS.Renderers.PickerRenderer))]
namespace Tojeero.iOS.Renderers
{	
	public class PickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Forms.Controls.Picker)this.Element;
			this.Control.TextColor = element.TextColor.ToUIColor();
			this.Control.BorderStyle = UIKit.UITextBorderStyle.None;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == Forms.Controls.Picker.TextColorProperty.PropertyName)
			{
				var element = (Forms.Controls.Picker)this.Element;
				this.Control.TextColor = element.TextColor.ToUIColor();
			}
		}
	}
}


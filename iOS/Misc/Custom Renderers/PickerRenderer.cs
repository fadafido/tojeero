using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(Tojeero.Forms.Picker), typeof(Tojeero.iOS.Renderers.PickerRenderer))]
namespace Tojeero.iOS.Renderers
{	
	public class PickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Tojeero.Forms.Picker)this.Element;
			this.Control.TextColor = element.TextColor.ToUIColor();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == Tojeero.Forms.Picker.TextColorProperty.PropertyName)
			{
				var element = (Tojeero.Forms.Picker)this.Element;
				this.Control.TextColor = element.TextColor.ToUIColor();
			}
		}
	}
}


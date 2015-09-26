using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly:ExportRenderer(typeof(Tojeero.Forms.Picker), typeof(Tojeero.Droid.Renderers.PickerRenderer))]
namespace Tojeero.Droid.Renderers
{	
	public class PickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Xamarin.Forms.Picker> e)
		{
			base.OnElementChanged(e);
			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Tojeero.Forms.Picker)this.Element;
			this.Control.SetTextColor(element.TextColor.ToAndroid());
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;
			
			if (e.PropertyName == Tojeero.Forms.Picker.TextColorProperty.PropertyName)
			{
				var element = (Tojeero.Forms.Picker)this.Element;
				this.Control.SetTextColor(element.TextColor.ToAndroid());
			}
		}
	}
}


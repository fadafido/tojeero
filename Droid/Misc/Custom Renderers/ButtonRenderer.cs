using System;
using Xamarin.Forms;

[assembly:ExportRenderer(typeof(Button), typeof(Tojeero.Droid.Renderers.ButtonRenderer))]

namespace Tojeero.Droid.Renderers
{	
	public class ButtonRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;

			this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == Button.IsEnabledProperty.PropertyName)
			{
				this.Element.Opacity = this.Element.IsEnabled ? 1f : 0.5f;
			}
		}
	}
}


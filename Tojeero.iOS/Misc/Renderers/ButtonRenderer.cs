using System;
using Xamarin.Forms;
using UIKit;

[assembly:ExportRenderer(typeof(Button), typeof(Tojeero.iOS.Renderers.ButtonRenderer))]

namespace Tojeero.iOS.Renderers
{	
	public class ButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Button> e)
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


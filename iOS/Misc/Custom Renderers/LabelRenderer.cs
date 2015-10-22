using System;
using UIKit;
using Tojeero.Forms;

[assembly:Xamarin.Forms.ExportRenderer(typeof(Label), typeof(Tojeero.iOS.Renderers.LabelRenderer))]

namespace Tojeero.iOS.Renderers
{	
	public class LabelRenderer : Xamarin.Forms.Platform.iOS.LabelRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;
			var element = (Label)this.Element;
			this.Control.Lines = element.LineCount;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;

			if (e.PropertyName == Tojeero.Forms.Label.LineCountProperty.PropertyName)
			{
				var element = (Label)this.Element;
				this.Control.Lines = element.LineCount;
			}
		}
	}
}


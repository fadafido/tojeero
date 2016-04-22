using System;
using UIKit;
using Tojeero.Forms;
using Tojeero.Forms.Controls;
using Xamarin.Forms;

[assembly:Xamarin.Forms.ExportRenderer(typeof(LabelEx), typeof(Tojeero.iOS.Renderers.LabelExRenderer))]

namespace Tojeero.iOS.Renderers
{	
	public class LabelExRenderer : Xamarin.Forms.Platform.iOS.LabelRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;
			updateLineCount();
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (this.Element == null || this.Control == null)
				return;
			updateLineCount();
		}

		void updateLineCount()
		{
			var element = (LabelEx)this.Element;
			this.Control.Lines = element.LineCount;
		}
	}
}


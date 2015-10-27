using System;
using Xamarin.Forms;
using Tojeero.Forms;

[assembly:ExportRenderer(typeof(LabelEx), typeof(Tojeero.Droid.Renderers.LabelExRenderer))]

namespace Tojeero.Droid.Renderers
{	
	public class LabelExRenderer : Xamarin.Forms.Platform.Android.LabelRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Label> e)
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

			if (e.PropertyName == LabelEx.LineCountProperty.PropertyName)
			{
				updateLineCount();
			}
		}

		private void updateLineCount()
		{
			var label = this.Element as LabelEx;
			this.Control.SetMaxLines(label.LineCount);
		}
	}
}


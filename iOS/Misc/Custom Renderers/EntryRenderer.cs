using System;
using Xamarin.Forms;
using UIKit;

[assembly:ExportRenderer(typeof(Entry), typeof(Tojeero.iOS.Renderers.EntryRenderer))]

namespace Tojeero.iOS.Renderers
{	
	public class EntryRenderer : Xamarin.Forms.Platform.iOS.EntryRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;

			this.Control.BorderStyle = UITextBorderStyle.None;
		}
	}
}


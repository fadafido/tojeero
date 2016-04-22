using System;
using Tojeero.iOS.Toolbox;
using Xamarin.Forms;
using UIKit;

[assembly:ExportRenderer(typeof(SearchBar), typeof(Tojeero.iOS.Renderers.SearchBarRenderer))]

namespace Tojeero.iOS.Renderers
{	
	public class SearchBarRenderer : Xamarin.Forms.Platform.iOS.SearchBarRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<SearchBar> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
				return;
			
			this.Control.InputAccessoryView = InputToolboxView.CreateFromNIB(null, InputAccessoryViewMode.Done);
		}
	}
}


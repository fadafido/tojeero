using System;
using Xamarin.Forms;
using Tojeero.Forms;
using UIKit;

[assembly:ExportRenderer(typeof(SearchBarEx), typeof(Tojeero.iOS.Renderers.SearchBarExRenderer))]

namespace Tojeero.iOS.Renderers
{
	public class SearchBarExRenderer : SearchBarRenderer
	{
		#region Private fields

		private UIImage _backgroundImage = null;

		#endregion
		
		#region Parent override

		protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<SearchBar> e)
		{
			base.OnElementChanged(e);
			if (this.Control == null || this.Element == null)
				return;
			updateBackground();
		}

		#endregion

		#region Utility methods

		private void updateBackground()
		{
			if (_backgroundImage == null)
			{
				var image = UIImage.FromFile("searchbarBackground.png");
				_backgroundImage = image.StretchableImage(10, 10);
				this.Control.BackgroundImage = _backgroundImage;
			}
		}

		#endregion
	}
}


using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (SearchBarEx), typeof (SearchBarExRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class SearchBarExRenderer : SearchBarRenderer
    {
        #region Private fields

        private UIImage _backgroundImage;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (Control == null || Element == null)
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
                Control.BackgroundImage = _backgroundImage;
            }
        }

        #endregion
    }
}
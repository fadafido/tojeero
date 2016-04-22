using System.ComponentModel;
using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (BorderView), typeof (BorderViewRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class BorderViewRenderer : ViewRenderer
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new UIView());
                }
                updateBorderWidth();
                updateBorderColor();
                updateBorderRadius();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Element == null || Control == null)
                return;
            if (e.PropertyName == BorderView.BorderWidthProperty.PropertyName)
            {
                updateBorderWidth();
            }
            else if (e.PropertyName == BorderView.BorderColorProperty.PropertyName)
            {
                updateBorderColor();
            }
            else if (e.PropertyName == BorderView.RadiusProperty.PropertyName)
            {
                updateBorderRadius();
            }
        }

        #endregion

        #region Utility methods

        private void updateBorderWidth()
        {
            var borderView = Element as BorderView;
            Control.Layer.BorderWidth = borderView.BorderWidth;
        }

        private void updateBorderColor()
        {
            var borderView = Element as BorderView;
            Control.Layer.BorderColor = borderView.BorderColor.ToUIColor().CGColor;
        }

        private void updateBorderRadius()
        {
            var borderView = Element as BorderView;
            Control.Layer.CornerRadius = borderView.Radius;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Tojeero.Forms;
using Tojeero.Forms.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly:ExportRenderer(typeof(StretchableImage), typeof(Tojeero.iOS.Renderers.StretchableImageRenderer))]
namespace Tojeero.iOS.Renderers
{
    public class StretchableImageRenderer : ViewRenderer<StretchableImage, UIImageView>
    {
        #region Parent override

        protected override async void OnElementChanged(ElementChangedEventArgs<StretchableImage> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (base.Control == null)
                {
                    var imageView = new UIImageView();
                    base.SetNativeControl(imageView);
                }
                updateImage();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == StretchableImage.PathProperty.PropertyName ||
                e.PropertyName == StretchableImage.CapsProperty.PropertyName)
            {
                updateImage();
            }
        }

        #endregion

        #region Utility methods

        private void updateImage()
        {
            UIImage stretchableImage = null;
            if (Element != null)
            {
                var image = Element.Path != null ? UIImage.FromFile(Element.Path) : null;
                stretchableImage = image?.StretchableImage((nint)Element.Caps.Width, (nint)Element.Caps.Height);
            }
            if (Control != null)
            {
                Control.Image = stretchableImage;
            }
        }

        #endregion
    }
}

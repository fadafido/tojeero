using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (ImageButton), typeof (ImageButtonRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class ImageButtonRenderer : ViewRenderer<ImageButton, UIButton>
    {
        #region Parent override

        protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new UIButton(UIButtonType.Custom));
                    Control.TouchUpInside += buttonTapped;
                }
                await UpdateBackgroundImage();
                await UpdateImage();
                await UpdateSelectedImage();
                UpdateIsEnabled();
                UpdateIsSelected();
            }
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == ImageButton.BackgroundImageProperty.PropertyName)
            {
                await UpdateBackgroundImage();
            }
            if (e.PropertyName == ImageButton.ImageProperty.PropertyName)
            {
                await UpdateImage();
            }
            if (e.PropertyName == ImageButton.SelectedImageProperty.PropertyName)
            {
                await UpdateSelectedImage();
            }
            if (e.PropertyName == ImageButton.IsSelectedProperty.PropertyName)
            {
                UpdateIsSelected();
            }
            if (e.PropertyName == ImageButton.IsEnabledProperty.PropertyName)
            {
                UpdateIsEnabled();
            }
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (Control != null)
            {
                Control.TouchUpInside -= buttonTapped;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Events

        private void buttonTapped(object sender, EventArgs eventArgs)
        {
            if (Element != null && Element.Command != null)
            {
                Element.Command.Execute(null);
            }
        }

        #endregion

        #region Utility methods

        private async Task UpdateBackgroundImage()
        {
            var btn = Element;
            await setImageAsync(btn.BackgroundImage, UIControlState.Normal, true);
        }

        private async Task UpdateImage()
        {
            var btn = Element;
            await setImageAsync(btn.Image, UIControlState.Normal);
        }

        private async Task UpdateSelectedImage()
        {
            var btn = Element;
            await setImageAsync(btn.SelectedImage, UIControlState.Selected);
        }

        private void UpdateIsEnabled()
        {
            Element.Opacity = Element.IsEnabled ? 1f : 0.5f;
        }

        private void UpdateIsSelected()
        {
            Control.Selected = Element.IsSelected;
        }

        private async Task setImageAsync(ImageSource source, UIControlState state = UIControlState.Normal,
            bool isBackground = false)
        {
            UIImage target = null;
            var handler = GetHandler(source);
            if (handler != null)
            {
                using (var image = await handler.LoadImageAsync(source))
                {
                    target = image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal);
                }
            }

            if (!isBackground)
                Control.SetImage(target, state);
            else
                Control.SetBackgroundImage(target, state);
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
                returnValue = new ImageLoaderSourceHandler();
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
                returnValue = new StreamImagesourceHandler();
            }
            return returnValue;
        }

        #endregion
    }
}
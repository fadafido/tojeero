using System.ComponentModel;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof (ImageButton), typeof (ImageButtonRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class ImageButtonRenderer : ViewRenderer<ImageButton, Android.Widget.ImageButton>
    {
        #region Private fields and properties

        private Bitmap _backgroundImage;
        private Bitmap _image;
        private Bitmap _selectedImage;

        #endregion

        #region Parent override

        protected override async void OnElementChanged(ElementChangedEventArgs<ImageButton> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                if (Control == null)
                {
                    var button = new Android.Widget.ImageButton(Context);
                    button.Touch += buttonTouched;
                    button.SetBackgroundColor(Color.Transparent);
                    button.Tag = this;
                    SetNativeControl(button);
                }
            }
            await UpdateBackgroundImage();
            await UpdateImage();
            UpdateIsEnabled();
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ImageButton.BackgroundImageProperty.PropertyName)
            {
                _backgroundImage = null;
            }

            if (e.PropertyName == ImageButton.ImageProperty.PropertyName)
            {
                _image = null;
            }

            if (e.PropertyName == ImageButton.SelectedImageProperty.PropertyName)
            {
                _selectedImage = null;
            }

            if (e.PropertyName == ImageButton.ImageProperty.PropertyName ||
                e.PropertyName == ImageButton.SelectedImageProperty.PropertyName ||
                e.PropertyName == ImageButton.IsSelectedProperty.PropertyName)
            {
                await UpdateImage();
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
            if (disposing && Control != null)
            {
                Control.Touch -= buttonTouched;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Events

        void buttonTouched(object sender, TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
                Control.Alpha = (float) Element.Opacity/2;
            else if (e.Event.Action == MotionEventActions.Up)
            {
                Control.Alpha = (float) Element.Opacity;
                if (Element != null && Element.Command != null)
                    Element.Command.Execute(null);
            }
        }

        #endregion

        #region Utility methods

        private async Task UpdateBackgroundImage()
        {
            var btn = Element;
            _backgroundImage = _backgroundImage != null ? _backgroundImage : await getBitmapAsync(btn.BackgroundImage);
            Control.Background = new BitmapDrawable(_backgroundImage);
        }

        private async Task UpdateImage()
        {
            var btn = Element;
            Bitmap target = null;
            if (!Element.IsSelected)
            {
                target = _image = _image != null ? _image : await getBitmapAsync(Element.Image);
            }
            else
            {
                target =
                    _selectedImage =
                        _selectedImage != null ? _selectedImage : await getBitmapAsync(Element.SelectedImage);
            }
            Control.SetImageBitmap(target);
        }

        private void UpdateIsEnabled()
        {
            Element.Opacity = Element.IsEnabled ? 1f : 0.5f;
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> for the supplied <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="source">The <see cref="ImageSource"/> to get the image for.</param>
        /// <returns>A loaded <see cref="Bitmap"/>.</returns>
        private async Task<Bitmap> getBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap) null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context);

            return returnValue;
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
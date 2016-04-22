using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Widget;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (StretchableImage), typeof (StretchableImageRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class StretchableImageRenderer : ViewRenderer<StretchableImage, ImageView>
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<StretchableImage> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                if (Control == null)
                {
                    var imageView = new ImageView(Context);
                    SetNativeControl(imageView);
                }
            }

            updateImage();
        }

        #endregion

        #region Utility methods

        private void updateImage()
        {
            NinePatchDrawable image = null;
            if (Element?.Path != null)
            {
                var resourceId = getResourceId();
                if (resourceId > 0)
                {
                    var sourceImage = BitmapFactory.DecodeResource(Resources, resourceId);
                    var chunk = sourceImage.GetNinePatchChunk();
                    image = new NinePatchDrawable(Resources, sourceImage, chunk, new Rect(), null);
                }
            }
            Control?.SetBackground(image);
        }

        private int getResourceId()
        {
            if (Element?.Path == null)
                return 0;
            var name = Element?.Path;
            var index = name.LastIndexOf(".");
            if (index >= 0)
                name = name.Remove(index).ToLower();
            try
            {
                var res = typeof (Resource.Drawable);
                var field = res.GetField(name);
                var drawableId = (int) field.GetValue(null);
                return drawableId;
            }
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}
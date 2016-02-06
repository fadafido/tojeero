using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio;
using Tojeero.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using ImageButton = Tojeero.Forms.ImageButton;

[assembly:ExportRenderer(typeof(StretchableImage), typeof(Tojeero.Droid.Renderers.StretchableImageRenderer))]
namespace Tojeero.Droid.Renderers
{
    public class StretchableImageRenderer : ViewRenderer<StretchableImage, Android.Widget.ImageView>
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<StretchableImage> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                if (base.Control == null)
                {
                    Android.Widget.ImageView imageView = new Android.Widget.ImageView(base.Context);
                    base.SetNativeControl(imageView);
                }
            }

            updateImage();
        }

        #endregion

        #region Utility methods

        private void updateImage()
        {
            NinePatchDrawable image = null;
            if ( Element?.Path != null)
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
            string name = Element?.Path;
            var index = name.LastIndexOf(".");
            if (index >= 0)
                name = name.Remove(index).ToLower();
            try
            {
                var res = typeof(Resource.Drawable);
                var field = res.GetField(name);
                int drawableId = (int)field.GetValue(null);
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
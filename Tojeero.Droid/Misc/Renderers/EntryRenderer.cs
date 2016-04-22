using System.Linq;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using EntryRenderer = Tojeero.Droid.Renderers.EntryRenderer;

[assembly: ExportRenderer(typeof (Entry), typeof (EntryRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class EntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            updateBackground();
        }

        #endregion

        #region Utility methods

        private void updateBackground()
        {
            var shape = new RoundRectShape(Enumerable.Repeat(5f, 8).ToArray(), null, null);
            var background = new ShapeDrawable(shape);
            background.Paint.Color = Color.White;
            Control.Background = background;
        }

        #endregion
    }
}
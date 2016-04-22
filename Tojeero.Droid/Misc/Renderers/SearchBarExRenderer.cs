using System.Linq;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Widget;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof (SearchBarEx), typeof (SearchBarExRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class SearchBarExRenderer : SearchBarRenderer
    {
        #region Parent Override

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (Control == null || Element == null)
                return;

            updateBackground();
            updateSearchBackground();
        }

        #endregion

        #region Utility methods

        private void updateBackground()
        {
            var shape = new RoundRectShape(Enumerable.Repeat(10f, 8).ToArray(), null, null);
            var background = new ShapeDrawable(shape);
            background.Paint.Color = Color.White;
            Control.Background = background;
        }

        private void updateSearchBackground()
        {
            var shape = new RectShape();
            var searchBackground = new ShapeDrawable(shape);
            searchBackground.Paint.Color = Color.White;

            var searchPlateId = Control.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
            var searchPlateView = Control.FindViewById(searchPlateId);
            if (searchPlateView != null)
            {
                searchPlateView.Background = searchBackground;
            }

            var textId = Control.Context.Resources.GetIdentifier("android:id/search_src_text", null, null);
            var textView = (TextView) Control.FindViewById(textId);
            if (textView != null)
            {
                textView.TextSize = 14;
                textView.LayoutParameters.Height = LayoutParams.MatchParent;
            }
        }

        #endregion
    }
}
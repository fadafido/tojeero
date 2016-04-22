using Android.Graphics.Drawables;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (BorderView), typeof (BorderViewRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class BorderViewRenderer : VisualElementRenderer<BorderView>
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                Background.Dispose();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BorderView> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null && e.OldElement == null)
            {
                UpdateBackground();
            }
        }

        private void UpdateBackground()
        {
            var strokeWidth = Xamarin.Forms.Forms.Context.ToPixels(Element.BorderWidth);
            var radius = Xamarin.Forms.Forms.Context.ToPixels(Element.Radius);
            var background = new GradientDrawable();
            background.SetColor(Element.BackgroundColor.ToAndroid());
            background.SetCornerRadius(radius);
            background.SetStroke((int) strokeWidth, Element.BorderColor.ToAndroid());
            Background = background;
        }
    }
}
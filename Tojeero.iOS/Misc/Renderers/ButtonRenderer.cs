using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ButtonRenderer = Tojeero.iOS.Renderers.ButtonRenderer;

[assembly: ExportRenderer(typeof (Button), typeof (ButtonRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class ButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            Element.Opacity = Element.IsEnabled ? 1f : 0.5f;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Button.IsEnabledProperty.PropertyName)
            {
                Element.Opacity = Element.IsEnabled ? 1f : 0.5f;
            }
        }
    }
}
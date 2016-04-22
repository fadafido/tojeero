using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using EntryRenderer = Tojeero.iOS.Renderers.EntryRenderer;

[assembly: ExportRenderer(typeof (Entry), typeof (EntryRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class EntryRenderer : Xamarin.Forms.Platform.iOS.EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}
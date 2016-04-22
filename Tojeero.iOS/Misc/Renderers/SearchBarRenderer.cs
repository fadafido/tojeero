using Tojeero.iOS.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SearchBarRenderer = Tojeero.iOS.Renderers.SearchBarRenderer;

[assembly: ExportRenderer(typeof (SearchBar), typeof (SearchBarRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class SearchBarRenderer : Xamarin.Forms.Platform.iOS.SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            Control.InputAccessoryView = InputToolboxView.CreateFromNIB(null, InputAccessoryViewMode.Done);
        }
    }
}
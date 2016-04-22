using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using WebViewRenderer = Tojeero.iOS.Renderers.WebViewRenderer;

[assembly: ExportRenderer(typeof (WebView), typeof (WebViewRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class WebViewRenderer : Xamarin.Forms.Platform.iOS.WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            ScrollView.ContentInset = new UIEdgeInsets(-64, 0, 0, 0);
        }
    }
}
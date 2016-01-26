using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(Tojeero.iOS.Renderers.WebViewRenderer))]
namespace Tojeero.iOS.Renderers
{
    public class WebViewRenderer : Xamarin.Forms.Platform.iOS.WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            this.ScrollView.ContentInset = new UIEdgeInsets(-64, 0, 0, 0);
        }
    }
}

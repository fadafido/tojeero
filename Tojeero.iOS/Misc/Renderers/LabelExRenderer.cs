using System.ComponentModel;
using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (LabelEx), typeof (LabelExRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class LabelExRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;
            updateLineCount();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;
            updateLineCount();
        }

        void updateLineCount()
        {
            var element = (LabelEx) Element;
            Control.Lines = element.LineCount;
        }
    }
}
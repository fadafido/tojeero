using System.ComponentModel;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (LabelEx), typeof (LabelExRenderer))]

namespace Tojeero.Droid.Renderers
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

            if (e.PropertyName == LabelEx.LineCountProperty.PropertyName)
            {
                updateLineCount();
            }
        }

        private void updateLineCount()
        {
            var label = Element as LabelEx;
            Control.SetMaxLines(label.LineCount > 0 ? label.LineCount : int.MaxValue);
        }
    }
}
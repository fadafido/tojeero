using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using PickerRenderer = Tojeero.iOS.Renderers.PickerRenderer;

[assembly: ExportRenderer(typeof (Tojeero.Forms.Controls.Picker), typeof (PickerRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class PickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;
            var element = (Forms.Controls.Picker) Element;
            Control.TextColor = element.TextColor.ToUIColor();
            Control.BorderStyle = UITextBorderStyle.None;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Forms.Controls.Picker.TextColorProperty.PropertyName)
            {
                var element = (Forms.Controls.Picker) Element;
                Control.TextColor = element.TextColor.ToUIColor();
            }
        }
    }
}
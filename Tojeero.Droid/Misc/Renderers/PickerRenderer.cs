using System.ComponentModel;
using System.Linq;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using PickerRenderer = Tojeero.Droid.Renderers.PickerRenderer;

[assembly: ExportRenderer(typeof (Tojeero.Forms.Controls.Picker), typeof (PickerRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class PickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || Element == null)
                return;
            var element = (Forms.Controls.Picker) Element;
            Control.SetTextColor(element.TextColor.ToAndroid());
            updateBackground();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Forms.Controls.Picker.TextColorProperty.PropertyName)
            {
                var element = (Forms.Controls.Picker) Element;
                Control.SetTextColor(element.TextColor.ToAndroid());
            }
        }

        #endregion

        #region Utility methods

        private void updateBackground()
        {
            var shape = new RoundRectShape(Enumerable.Repeat(5f, 8).ToArray(), null, null);
            var background = new ShapeDrawable(shape);
            background.Paint.Color = Color.White;
            Control.Background = background;
        }

        #endregion
    }
}
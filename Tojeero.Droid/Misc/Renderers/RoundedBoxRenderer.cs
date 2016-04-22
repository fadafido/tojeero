using System.ComponentModel;
using Android.Graphics;
using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof (RoundedBox), typeof (RoundedBoxRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class RoundedBoxRenderer : BoxRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            SetWillNotDraw(false);

            Invalidate();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == RoundedBox.CornerRadiusProperty.PropertyName)
            {
                Invalidate();
            }
        }

        public override void Draw(Canvas canvas)
        {
            var box = Element as RoundedBox;
            var rect = new Rect();
            var paint = new Paint
            {
                Color = box.BackgroundColor.ToAndroid(),
                AntiAlias = true
            };

            GetDrawingRect(rect);

            var radius = (float) (rect.Width()/box.Width*box.CornerRadius);

            canvas.DrawRoundRect(new RectF(rect), radius, radius, paint);
        }
    }
}
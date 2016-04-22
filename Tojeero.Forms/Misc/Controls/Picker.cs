using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class Picker : Xamarin.Forms.Picker
    {
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<Picker, Color>(p => p.TextColor, Color.Red);

        public Color TextColor
        {
            get { return (Color) GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }
    }
}
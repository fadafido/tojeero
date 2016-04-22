using Xamarin.Forms;

namespace Tojeero.Forms.Controls
{
    public class ListView : Xamarin.Forms.ListView
    {
        public static BindableProperty FooterViewProperty = BindableProperty.Create<ListView, View>(o => o.FooterView,
            null);

        public View FooterView
        {
            get { return (View) GetValue(FooterViewProperty); }
            set { SetValue(FooterViewProperty, value); }
        }
    }
}
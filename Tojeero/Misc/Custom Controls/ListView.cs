using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tojeero.Forms
{
    public class ListView : Xamarin.Forms.ListView
    {
        public ListView()
        {

        }

        public static BindableProperty FooterViewProperty = BindableProperty.Create<ListView, View>(o => o.FooterView, null);

        public View FooterView
        {
            get { return (View) GetValue(FooterViewProperty); }
            set { SetValue(FooterViewProperty, value); }
        }
    }
}

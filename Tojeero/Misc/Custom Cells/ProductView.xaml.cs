using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Tojeero.Forms
{
    public partial class ProductView : Grid
    {
        public ProductView()
        {
            InitializeComponent();
        }

        public Image ProductImage => productImage;
    }
}

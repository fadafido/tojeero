using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Tojeero.Forms
{
    public class StretchableImage : View
    {
        #region Properties

        public static BindableProperty PathProperty = BindableProperty.Create<StretchableImage, string>(o => o.Path, "");

        public string Path
        {
            get { return (string) GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }


        public static BindableProperty CapsProperty = BindableProperty.Create<StretchableImage, Size>(o => o.Caps, Size.Zero);

        public Size Caps
        {
            get { return (Size) GetValue(CapsProperty); }
            set { SetValue(CapsProperty, value); }
        }

        #endregion
    }
}

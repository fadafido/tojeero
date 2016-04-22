using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            return string.Format("{0:C}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
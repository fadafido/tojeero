using System;
using System.Globalization;
using Tojeero.Core.Toolbox;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
    public class NumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            decimal number;
            if (!decimal.TryParse(value.ToString(), out number))
                return "";
            return number.GetShortString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
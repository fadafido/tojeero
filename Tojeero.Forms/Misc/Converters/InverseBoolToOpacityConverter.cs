using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
    public class InverseBoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? 0.5 : 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) value >= 1 ? false : true;
        }
    }
}
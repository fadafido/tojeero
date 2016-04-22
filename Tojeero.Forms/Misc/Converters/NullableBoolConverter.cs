using System;
using System.Globalization;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
    public class NullableBoolConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return boolValue != null ? boolValue.Value : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            return boolValue;
        }

        #endregion
    }
}
using System;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
	public class BoolToOpacityConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? 1 : 0.5;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (double)value >= 1 ? true : false;
		}

		#endregion
		
	}
}


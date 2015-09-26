using System;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class InverseBoolToOpacityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? 0.5 : 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (double)value >= 1 ? false : true;
		}
	}
}


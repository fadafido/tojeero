using System;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class DoubleConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double)
				return value.ToString ();
			return value;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double dbl;
			if (double.TryParse (value as string, out dbl))
				return dbl;
			return value;
		}
	}
}


using System;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class NumberToStringConverter : IValueConverter
	{
		private const int THOUSAND = 1000;
		private const int MILLION = 1000000;
		private const int BILLION = 1000000000;
		private const long TRILLION = 1000000000000;

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return "";
			decimal number;
			if (!decimal.TryParse(value.ToString(), out number))
				return "";

			if (number < THOUSAND)
				return number.ToString("n0");
			else if (number < MILLION)
				return (number / THOUSAND).ToString("n0") + "k";
			else if (number < BILLION)
				return (number / MILLION).ToString("n0") + "m";
			else if (number < TRILLION)
				return (number / BILLION).ToString("n0") + "b";
			else
				return "999b+";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}


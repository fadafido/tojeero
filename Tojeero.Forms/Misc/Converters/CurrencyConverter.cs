using System;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
{
	public class CurrencyConverter : IValueConverter
	{
		#region IValueConverter implementation
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value == null)
				return "";
			return string.Format("{0:C}", value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
		#endregion
		
	}
}


using System;

namespace Tojeero.Forms.Converters
{

	public class InverseVisibilityConverter : VisibilityConverter
	{
		public override object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !(bool)base.Convert(value, targetType, parameter, culture);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !(bool)base.ConvertBack(value, targetType, parameter, culture);
		}
	}
}

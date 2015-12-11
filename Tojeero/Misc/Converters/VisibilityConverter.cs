using System;
using Xamarin.Forms;
using System.Collections;

namespace Tojeero.Forms
{
	public class VisibilityConverter : IValueConverter
	{
		public virtual object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value == null)
				return false;
			if (value is ICollection)
			{
				var collection = value as ICollection;
				return collection.Count != 0;
			}
			if (string.IsNullOrEmpty(value.ToString()))
				return false;
			return true;
		}

		public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}


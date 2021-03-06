﻿using System;
using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class NullableBoolConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var boolValue = value as bool?;
			return boolValue != null ? boolValue.Value : false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var boolValue = value as bool?;
			return boolValue;
		}

		#endregion
		
	}
}


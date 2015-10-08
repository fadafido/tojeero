using System;
using Xamarin.Forms;
using System.Globalization;
using Tojeero.Core.Services;
using Cirrious.CrossCore;

namespace Tojeero.Forms
{
	public class LanguageToCultureConverter : IValueConverter
	{
		#region IValueConverter implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
		{
			var language = value as LanguageCode?;
			if (language == null)
				return null;
			var localizationService = Mvx.Resolve<ILocalizationService>();
			return localizationService.GetNativeLanguageName(language.Value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
		{
			throw new NotImplementedException();
		}

		#endregion
		
	}
}


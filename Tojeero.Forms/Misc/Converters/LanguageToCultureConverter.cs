using System;
using System.Globalization;
using Cirrious.CrossCore;
using Tojeero.Core.Model;
using Tojeero.Core.Services.Contracts;
using Xamarin.Forms;

namespace Tojeero.Forms.Converters
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


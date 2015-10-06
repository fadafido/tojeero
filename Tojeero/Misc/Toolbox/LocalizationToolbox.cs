using System;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core
{
	public static class LocalizationToolbox
	{
		private const string DefaultCulture = "en";

		public static ILocalizationService _localizationService;
		public static ILocalizationService LocalizationService
		{
			get
			{
				if (_localizationService == null)
				{
					_localizationService = Mvx.Resolve<ILocalizationService>();
				}
				return _localizationService;
			}
		}

		public static string LocalizedValue(this IDictionary<string, string> values)
		{
			if (values == null || values.Keys.Count == 0)
				return "";
			string value;
			string culture = LocalizationService.Culture.TwoLetterISOLanguageName;
			if (!values.TryGetValue(culture, out value))
			{
				if (!values.TryGetValue(DefaultCulture, out value))
					value = values[values.Keys.First()];
			}
			return value;
		}
	}
}


using System;
using System.Runtime.CompilerServices;
using Tojeero.Core.Services;

namespace Tojeero.Droid
{
	public class LocalizationService : ILocalizationService
	{
		public System.Globalization.CultureInfo GetCurrentCultureInfo ()
		{
			var androidLocale = Java.Util.Locale.Default;
			var netLanguage = androidLocale.ToString().Replace ("_", "-"); // turns pt_BR into pt-BR
			return new System.Globalization.CultureInfo(netLanguage);
		}
	}
}


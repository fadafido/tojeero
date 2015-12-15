using System;
using Xamarin.Forms;
using Tojeero.Core.Services;
using Foundation;

namespace Tojeero.iOS
{
	public class LocalizationService : BaseLocalizationService
	{
		#region Constructors

		public LocalizationService()
			: base()
		{
		}

		#endregion
		
		#region Utility methods

		protected override System.Globalization.CultureInfo getSystemCultureInfo()
		{
			var netLanguage = "en";
			var prefLanguageOnly = "en";
			if (NSLocale.PreferredLanguages.Length > 0)
			{
				var pref = NSLocale.PreferredLanguages[0];
				prefLanguageOnly = pref.Substring(0, 2);
				if (prefLanguageOnly == "pt")
				{
					if (pref == "pt")
						pref = "pt-BR"; // get the correct Brazilian language strings from the PCL RESX (note the local iOS folder is still "pt")
					else
						pref = "pt-PT"; // Portugal
				}
				netLanguage = pref.Replace("_", "-");
				Console.WriteLine("preferred language:" + netLanguage);
			}
			System.Globalization.CultureInfo ci = null;
			try
			{
				ci = new System.Globalization.CultureInfo(netLanguage);
			}
			catch
			{
				// iOS locale not valid .NET culture (eg. "en-ES" : English in Spain)
				// fallback to first characters, in this case "en"
				ci = new System.Globalization.CultureInfo(prefLanguageOnly);
			}
			return ci;
		}

		#endregion
	}
}


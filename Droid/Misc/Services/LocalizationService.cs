using System;
using System.Runtime.CompilerServices;
using Tojeero.Core.Services;
using System.Globalization;
using Tojeero.Core.Toolbox;

namespace Tojeero.Droid
{
	public class LocalizationService : BaseLocalizationService
	{
		#region Constructors

		public LocalizationService()
			: base()
		{
		}

		#endregion

		#region Utilit Methods

		protected override CultureInfo getSystemCultureInfo ()
		{
			var androidLocale = Java.Util.Locale.Default;
			var netLanguage = androidLocale.ToString().Replace ("_", "-"); // turns pt_BR into pt-BR
			return new System.Globalization.CultureInfo(netLanguage);
		}
			
		#endregion
	}
}


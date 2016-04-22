using System.Globalization;
using Java.Util;
using Tojeero.Core.Services;

namespace Tojeero.Droid.Services
{
    public class LocalizationService : BaseLocalizationService
    {
        #region Constructors

        #endregion

        #region Utilit Methods

        protected override CultureInfo getSystemCultureInfo()
        {
            var androidLocale = Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-"); // turns pt_BR into pt-BR
            return new CultureInfo(netLanguage);
        }

        #endregion
    }
}
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels.User
{
    public class TermsViewModel : MvxViewModel
    {
        #region Private fields and properties

        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public TermsViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        #endregion

        #region Properties

        public string TermsUrl
        {
            get
            {
                var language = _localizationService.Language.GetString();
                var url = string.Format("http://tojeero.com/{0}/mobiletermofuse?key={1}", language,
                    Constants.BackendRequestKey);
                return url;
            }
        }

        #endregion
    }
}
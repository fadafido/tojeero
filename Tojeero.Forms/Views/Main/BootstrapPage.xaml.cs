using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Main;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
{
    public partial class BootstrapPage
    {
        #region Constructors

        public BootstrapPage()
        {
            InitializeComponent();

            ViewModel = MvxToolbox.LoadViewModel<BootstrapViewModel>();

            SetupPickers();
        }

        #endregion

        #region Utility methods

        private void SetupPickers()
        {
            countriesPicker.FacetsLoader = ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => ViewModel.Countries);

            citiesPicker.FacetsLoader = ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => ViewModel.Country?.Cities);

            var localizationService = Mvx.Resolve<ILocalizationService>();
            languagesPicker.StringFormat = language => { return localizationService.GetNativeLanguageName(language); };
        }

        #endregion
    }
}
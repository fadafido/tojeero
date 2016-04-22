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
    public partial class BootstrapPage : ContentPage
    {
        #region Properties

        private BootstrapViewModel _viewModel;

        public BootstrapViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindingContext = _viewModel;
                }
            }
        }

        #endregion

        #region Constructors

        public BootstrapPage()
        {
            ViewModel = MvxToolbox.LoadViewModel<BootstrapViewModel>();
            InitializeComponent();

            setupPickers();
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.BootstrapCommand.Execute(null);
        }

        #endregion

        #region Utility methods

        private void setupPickers()
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
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
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					_viewModel = value;
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region Constructors

		public BootstrapPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<BootstrapViewModel>();
			InitializeComponent();
            
            setupPickers();
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.BootstrapCommand.Execute(null);
		}

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            countriesPicker.FacetsLoader = this.ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => this.ViewModel.Countries);

            citiesPicker.FacetsLoader = this.ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => this.ViewModel.Country?.Cities);

            var localizationService = Mvx.Resolve<ILocalizationService>();
            this.languagesPicker.StringFormat = (language) =>
            {
                return localizationService.GetNativeLanguageName(language);
            };
        }

        #endregion
    }
}


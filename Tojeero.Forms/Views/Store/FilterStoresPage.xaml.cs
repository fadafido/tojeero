using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class FilterStoresPage : ContentPage
    {
        #region Constructors

        public FilterStoresPage(string query = null)
        {
            ViewModel = MvxToolbox.LoadViewModel<FilterStoresViewModel>();
            ViewModel.Query = query;
            InitializeComponent();
            setupPickers();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
            {
                await Navigation.PopModalAsync();
                ViewModel.DoneCommand.Execute(null);
            }));
        }

        #endregion

        #region Properties

        private FilterStoresViewModel _viewModel;

        public FilterStoresViewModel ViewModel
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

        #region View Lifecycle

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
            await ViewModel.ReloadCount();
        }

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            countriesPicker.FacetsLoader = ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => ViewModel.Countries);

            citiesPicker.FacetsLoader = ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => ViewModel.Cities);

            categoriesPicker.FacetsLoader = ViewModel.FetchCategoryFacets;
            categoriesPicker.ObjectsLoader =
                () => Task<IList<IStoreCategory>>.Factory.StartNew(() => ViewModel.Categories);
        }

        #endregion
    }
}
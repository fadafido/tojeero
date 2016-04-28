using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class FilterStoresPage
    {
        #region Private fields

        private readonly string _query;

        #endregion


        #region Constructors

        public FilterStoresPage(string query = null)
        {
            InitializeComponent();

            _query = query;
            ViewModel = MvxToolbox.LoadViewModel<FilterStoresViewModel>();
            
            SetupPickers();
            ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
            {
                await Navigation.PopModalAsync();
                ViewModel.DoneCommand.Execute(null);
            }));
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.Query = _query;
        }

        #endregion

        #region Utility methods

        private void SetupPickers()
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
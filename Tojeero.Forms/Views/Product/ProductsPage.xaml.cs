using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class ProductsPage
    {

        #region Constructors

        public ProductsPage()
        {
            InitializeComponent();

            ProductsButton.IsSelected = true;
            StoresButton.IsSelected = false;

            var vm = MvxToolbox.LoadViewModel<ProductsViewModel>();
            vm.ShowFiltersAction =
                async () =>
                {
                    await Navigation.PushModalAsync(new NavigationPage(new FilterProductsPage(ViewModel.SearchQuery)));
                };
            vm.ShowProductDetailsAction = async p =>
            {
                var productDetails = new ProductDetailsPage(p);
                await Navigation.PushAsync(productDetails);
            };
            vm.ChangeListModeAction = mode => { UpdateListLayout(mode); };
            ViewModel = vm;
            SearchBar.Placeholder = AppResources.PlaceholderSearchProducts;
            UpdateListLayout(vm.ListMode);
        }

        #endregion


        #region Utility methods

        private void UpdateListLayout(ListMode mode)
        {
            ListViewEx.RowHeight = mode == ListMode.Normal ? 100 : 350;
            var type = mode == ListMode.Normal ? typeof (ProductListCell) : typeof (ProductListLargeCell);
            ListViewEx.ItemTemplate = new DataTemplate(type);
        }

        #endregion
    }
}
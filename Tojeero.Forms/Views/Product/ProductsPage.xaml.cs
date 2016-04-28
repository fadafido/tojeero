using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class ProductsPage : BaseSearchableTabPage
    {
        #region Properties

        public new ProductsViewModel ViewModel
        {
            get { return base.ViewModel as ProductsViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Constructors

        public ProductsPage()
        {
            InitializeComponent();

            ProductsButton.IsSelected = true;
            StoresButton.IsSelected = false;

            ViewModel = MvxToolbox.LoadViewModel<ProductsViewModel>();
            ViewModel.ShowFiltersAction =
                async () =>
                {
                    await Navigation.PushModalAsync(new NavigationPage(new FilterProductsPage(ViewModel.SearchQuery)));
                };

            ViewModel.ChangeListModeAction = mode => { updateListLayout(mode); };
            SearchBar.Placeholder = AppResources.PlaceholderSearchProducts;
            ListViewEx.ItemSelected += itemSelected;
            updateListLayout(ViewModel.ListMode);
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
        }

        #endregion

        #region UI Events

        private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = ((ListViewEx) sender).SelectedItem as ProductViewModel;
            if (item != null)
            {
                ((ListViewEx) sender).SelectedItem = null;
                var productDetails = new ProductDetailsPage(item.Product);
                await Navigation.PushAsync(productDetails);
            }
        }

        #endregion

        #region Utility methods

        private void updateListLayout(ListMode mode)
        {
            ListViewEx.RowHeight = mode == ListMode.Normal ? 100 : 350;
            var type = mode == ListMode.Normal ? typeof (ProductListCell) : typeof (ProductListLargeCell);
            ListViewEx.ItemTemplate = new DataTemplate(type);
        }

        #endregion
    }
}
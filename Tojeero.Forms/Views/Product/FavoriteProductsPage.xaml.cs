using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class FavoriteProductsPage : BaseCollectionPage
    {
        #region Constructors

        public FavoriteProductsPage()
        {
            InitializeComponent();
            ViewModel = MvxToolbox.LoadViewModel<FavoriteProductsViewModel>();
            ListViewEx.ItemSelected += itemSelected;
        }

        #endregion

        #region Properties

        public new FavoriteProductsViewModel ViewModel
        {
            get { return base.ViewModel as FavoriteProductsViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadFirstPageCommand.Execute(null);
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
    }
}
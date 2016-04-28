using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class FavoriteProductsPage
    {
        #region Constructors

        public FavoriteProductsPage()
        {
            InitializeComponent();
            var vm = MvxToolbox.LoadViewModel<FavoriteProductsViewModel>();
            vm.ShowProductDetailsAction = async p =>
            {
                var productDetails = new ProductDetailsPage(p);
                await Navigation.PushAsync(productDetails);
            };
            ViewModel = vm;
        }

        #endregion

    }
}
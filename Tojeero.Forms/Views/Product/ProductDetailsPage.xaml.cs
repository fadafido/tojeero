using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Chat;
using Tojeero.Forms.Views.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Product
{
    public partial class ProductDetailsPage
    {
        #region Private fields

        private readonly IProduct _product;
        private readonly ContentMode _mode;

        #endregion


        #region Constructors

        public ProductDetailsPage(IProduct product, ContentMode mode = ContentMode.View)
        {
            InitializeComponent();

            _mode = mode;
            _product = product;
            ViewModel = MvxToolbox.LoadViewModel<ProductDetailsViewModel>();
            
            carouselLayout.IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots;
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.Product = _product;
            ViewModel.Mode = _mode;

            ViewModel.ShowStoreInfoPageAction = async s => { await Navigation.PushAsync(new StoreInfoPage(s)); };

            if (_mode == ContentMode.Edit)
            {
                ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
                {
                    var saveProductPage = new SaveProductPage(_product, _product.Store);
                    await Navigation.PushModalAsync(new NavigationPage(saveProductPage));
                }));
            }
            ViewModel.ShowChatPageAction = async channel =>
            {
                var chatPage = new ChatPage(channel, ViewModel.Product);
                await Navigation.PushAsync(chatPage);
            };
        }

        #endregion
    }
}
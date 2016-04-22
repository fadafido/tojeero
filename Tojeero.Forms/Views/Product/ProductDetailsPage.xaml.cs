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
    public partial class ProductDetailsPage : ContentPage
    {
        #region Constructors

        public ProductDetailsPage(IProduct product, ContentMode mode = ContentMode.View)
        {
            ViewModel = MvxToolbox.LoadViewModel<ProductDetailsViewModel>();
            ViewModel.Product = product;
            ViewModel.Mode = mode;
            InitializeComponent();
            ViewModel.ShowStoreInfoPageAction = async s => { await Navigation.PushAsync(new StoreInfoPage(s)); };

            if (mode == ContentMode.Edit)
            {
                ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
                {
                    var saveProductPage = new SaveProductPage(product, product.Store);
                    await Navigation.PushModalAsync(new NavigationPage(saveProductPage));
                }));
            }
            ViewModel.ShowChatPageAction = async channel =>
            {
                var chatPage = new ChatPage(channel, ViewModel.Product);
                await Navigation.PushAsync(chatPage);
            };
            carouselLayout.IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots;
        }

        #endregion

        #region Properties

        private ProductDetailsViewModel _viewModel;

        public ProductDetailsViewModel ViewModel
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

        #region Parent 

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
        }

        #endregion
    }
}
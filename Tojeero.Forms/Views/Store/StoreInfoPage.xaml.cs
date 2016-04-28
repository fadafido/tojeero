using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Views.Main;
using Tojeero.Forms.Views.Product;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class StoreInfoPage
    {
        #region Private fields and properties

        private bool _toolbarButtonsAdded;
        private readonly IStore _store;
        private readonly ContentMode _mode;

        #endregion

        #region Constructors

        public StoreInfoPage(IStore store, ContentMode mode = ContentMode.View)
        {
            InitializeComponent();

            _mode = mode;
            _store = store;
            //Setup view model
            ViewModel = MvxToolbox.LoadViewModel<StoreInfoViewModel>();
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.Store = _store;
            ViewModel.Mode = _mode;

            //Setup Header
            HeaderView.BindingContext = ViewModel;

            //Setup events

            ViewModel.Products.ReloadFinished += (sender, e) => { listView.EndRefresh(); };

            //Setup view model actions
            ViewModel.ShowStoreDetailsAction = async s => { await Navigation.PushAsync(new StoreDetailsPage(s, _mode)); };

            ViewModel.ShowProductDetailsAction = async (p, m) =>
            {
                var productDetails = new ProductDetailsPage(p, m);
                await Navigation.PushAsync(productDetails);
            };

            ViewModel.AddProductAction =
                async (p, s) => { await Navigation.PushModalAsync(new NavigationPage(new SaveProductPage(p, s))); };
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_toolbarButtonsAdded)
            {
                if (ViewModel.Mode == ContentMode.Edit)
                {
                    if (ViewModel.Store != null)
                    {
                        ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
                        {
                            var editStorePage = new SaveStorePage(ViewModel.Store);
                            await Navigation.PushModalAsync(new NavigationPage(editStorePage));
                        }));
                    }
                }

                //if this view is not inside root page add close button
                var root = this.FindParent<RootPage>();
                if (root == null)
                {
                    ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "",
                        async () => { await Navigation.PopModalAsync(); }, priority: 15));
                }
                _toolbarButtonsAdded = true;
            }
        }

        #endregion

    }
}
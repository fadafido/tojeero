using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Views.Main;
using Tojeero.Forms.Views.Product;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Store
{
    public partial class StoreInfoPage : ContentPage
    {
        #region Private fields and properties

        private bool _toolbarButtonsAdded;

        #endregion

        #region Constructors

        public StoreInfoPage(IStore store, ContentMode mode = ContentMode.View)
        {
            //Setup view model
            ViewModel = MvxToolbox.LoadViewModel<StoreInfoViewModel>();
            ViewModel.Store = store;
            ViewModel.Mode = mode;

            InitializeComponent();

            //Setup Header
            HeaderView.BindingContext = ViewModel;

            //Setup events
            listView.ItemSelected += itemSelected;
            ViewModel.Products.ReloadFinished += (sender, e) => { listView.EndRefresh(); };

            //Setup view model actions
            ViewModel.ShowStoreDetailsAction = async s => { await Navigation.PushAsync(new StoreDetailsPage(s, mode)); };

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
            ViewModel.Products.LoadFirstPageCommand.Execute(null);
            ViewModel.ReloadCommand.Execute(null);
        }

        #endregion

        #region Properties

        private StoreInfoViewModel _viewModel;

        public StoreInfoViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    if (HeaderView != null)
                        HeaderView.BindingContext = _viewModel;
                    BindingContext = _viewModel;
                }
            }
        }

        #endregion

        #region UI Events

        private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = ((ListView) sender).SelectedItem as ProductViewModel;
            if (item != null)
            {
                ((ListView) sender).SelectedItem = null;
                var productDetails = new ProductDetailsPage(item.Product, ViewModel.Mode);
                await Navigation.PushAsync(productDetails);
            }
        }

        #endregion
    }
}
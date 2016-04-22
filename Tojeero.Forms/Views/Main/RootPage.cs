using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Main;
using Tojeero.Forms.Views.Product;
using Tojeero.Forms.Views.Store;
using Tojeero.Forms.Views.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
{
    public class RootPage : MasterDetailPage
    {
        #region Private fields

        private bool _wasUserStoreShown;
        private readonly TabbedPage _tabs;
        private NavigationPage _userStorePage;

        private NavigationPage _productsPage;

        private NavigationPage ProductsPage
        {
            get
            {
                if (_productsPage == null)
                {
                    _productsPage = new NavigationPage(new ProductsPage());
                    _productsPage.Icon = "shopIcon.png";
                    _productsPage.Title = AppResources.TitleShop;
                }
                return _productsPage;
            }
        }

        private NavigationPage _storesPage;

        private NavigationPage StoresPage
        {
            get
            {
                if (_storesPage == null)
                {
                    _storesPage = new NavigationPage(new StoresPage());
                    _storesPage.Icon = "shopIcon.png";
                    _storesPage.Title = AppResources.TitleShop;
                }
                return _storesPage;
            }
        }

        private NavigationPage _favoritesPage;

        private NavigationPage FavoritesPage
        {
            get
            {
                if (_favoritesPage == null)
                {
                    _favoritesPage = new NavigationPage(new FavoritesPage());
                    _favoritesPage.Icon = "favoritesIcon.png";
                    _favoritesPage.Title = AppResources.TitleFavorites;
                }
                return _favoritesPage;
            }
        }

        #endregion

        #region Constructors

        public RootPage()
        {
            ViewModel = MvxToolbox.LoadViewModel<RootViewModel>();
            Master = new SideMenuPage
            {
                Title = AppResources.AppName
            };

            _tabs = new TabbedPage();
            _tabs.Children.Add(ProductsPage);

            Detail = _tabs;

            ViewModel.Initialize();
        }

        #endregion

        #region Public API

        private RootViewModel _viewModel;

        public RootViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    setupViewModel();
                }
            }
        }

        public void SelectProductsPage()
        {
            if (_productsPage == null || _tabs.CurrentPage != _productsPage)
            {
                if (_storesPage != null)
                    _tabs.Children.Remove(_storesPage);
                _tabs.Children.Insert(0, ProductsPage);
                _tabs.CurrentPage = ProductsPage;
            }
        }

        public void SelectStoresPage()
        {
            if (_storesPage == null || _tabs.CurrentPage != _storesPage)
            {
                if (_productsPage != null)
                    _tabs.Children.Remove(_productsPage);
                _tabs.Children.Insert(0, StoresPage);
                _tabs.CurrentPage = StoresPage;
            }
        }

        #endregion

        #region Utility methods

        private void showFavorites()
        {
            if (!_tabs.Children.Contains(FavoritesPage))
                _tabs.Children.Add(FavoritesPage);
        }

        private void hideFavorites()
        {
            if (_favoritesPage != null)
            {
                _tabs.Children.Remove(_favoritesPage);
                _favoritesPage = null;
            }
        }

        private void showUserStore(IStore store)
        {
            if (_userStorePage == null)
            {
                if (store == null)
                {
                    _userStorePage = new NavigationPage(new SaveStorePage(null));
                    _userStorePage.Icon = "createUserStoreIcon.png";
                }
                else
                {
                    _userStorePage = new NavigationPage(new StoreInfoPage(store, ContentMode.Edit));
                    _userStorePage.Icon = "userStoreIcon.png";
                }
                _userStorePage.Title = ViewModel.UserStoreViewModel.ShowSaveStoreTitle;
                _tabs.Children.Add(_userStorePage);
                if (_wasUserStoreShown)
                    _tabs.CurrentPage = _userStorePage;
            }
        }

        private void hideUserStore()
        {
            if (_userStorePage != null)
            {
                if (_tabs.CurrentPage == _userStorePage)
                    _wasUserStoreShown = true;
                _tabs.Children.Remove(_userStorePage);
                _userStorePage = null;
            }
        }

        private void setupViewModel()
        {
            ViewModel.ShowFavorites = showFavorites;
            ViewModel.HideFavorites = hideFavorites;
            ViewModel.UserStoreViewModel.DidLoadUserStoreAction = showUserStore;
            ViewModel.UserStoreViewModel.IsLoadingStoreAction = hideUserStore;
            BindingContext = _viewModel;
        }

        #endregion
    }
}
using System;

using Xamarin.Forms;
using Tojeero.Core.Services;
using Cirrious.CrossCore;

using Tojeero.Forms.Resources;
using Tojeero.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using System.Threading.Tasks;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		#region Private fields

		private bool _wasUserStoreShown;
		private TabbedPage _tabs;
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
			this.ViewModel = MvxToolbox.LoadViewModel<RootViewModel>();
			this.Master = new SideMenuPage()
			{
				Title = AppResources.AppName
			};
						
			_tabs = new TabbedPage();
			_tabs.Children.Add(ProductsPage);

			this.Detail = _tabs;

			this.ViewModel.Initialize();
		}

		#endregion

		#region Public API

		private RootViewModel _viewModel;

		public RootViewModel ViewModel
		{ 
			get
			{
				return _viewModel; 
			}
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
				if(_storesPage != null)
					_tabs.Children.Remove(_storesPage);
				_tabs.Children.Insert(0, this.ProductsPage);
				_tabs.CurrentPage = ProductsPage;
			}
		}

		public void SelectStoresPage()
		{
			if (_storesPage == null || _tabs.CurrentPage != _storesPage)
			{
				if(_productsPage != null)
					_tabs.Children.Remove(_productsPage);
				_tabs.Children.Insert(0, this.StoresPage);
				_tabs.CurrentPage = StoresPage;
			}
		}

		#endregion

		#region Utility methods

		private void showFavorites()
		{
			if(!_tabs.Children.Contains(FavoritesPage))
				_tabs.Children.Add(FavoritesPage);
		}

		private void hideFavorites()
		{
			if(_favoritesPage != null)
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
					_userStorePage = new NavigationPage(new SaveStorePage(null));
				else
					_userStorePage = new NavigationPage(new StoreInfoPage(store, ContentMode.Edit));
				_userStorePage.Title = this.ViewModel.UserStoreViewModel.ShowSaveStoreTitle;
				_tabs.Children.Add(_userStorePage);
				if (_wasUserStoreShown)
					_tabs.CurrentPage = _userStorePage;
			}
		}

		private void hideUserStore()
		{
			if(_userStorePage != null)
			{
				if(_tabs.CurrentPage == _userStorePage)
					_wasUserStoreShown = true;			
				_tabs.Children.Remove(_userStorePage);
				_userStorePage = null;
			}
		}

		private void setupViewModel()
		{			
			this.ViewModel.ShowFavorites = showFavorites;
			this.ViewModel.HideFavorites = hideFavorites;
			this.ViewModel.UserStoreViewModel.DidLoadUserStoreAction = showUserStore;
			this.ViewModel.UserStoreViewModel.IsLoadingStoreAction = hideUserStore;
			this.BindingContext = _viewModel;
		}

		#endregion
	}
}



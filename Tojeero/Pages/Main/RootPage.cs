using System;

using Xamarin.Forms;
using Tojeero.Core.Services;
using Cirrious.CrossCore;

using Tojeero.Forms.Resources;
using Tojeero.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		#region Private fields

		private TabbedPage _tabs;
		private readonly IAuthenticationService _authService;
		private readonly MvxSubscriptionToken _sessionChangedToken;

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
			_authService = Mvx.Resolve<IAuthenticationService>();
			var messenger = Mvx.Resolve<IMvxMessenger>();
			_sessionChangedToken = messenger.SubscribeOnMainThread<SessionStateChangedMessage>((m) =>
				{
					reloadFavorites();
				});
			this.Master = new SideMenuPage()
			{
				Title = AppResources.AppName
			};
						
			_tabs = new TabbedPage();
			_tabs.Children.Add(ProductsPage);

			reloadFavorites();

			this.Detail = _tabs;
		}

		#endregion

		#region Public API

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

		void reloadFavorites()
		{
			if (_authService.State == SessionState.LoggedIn)
			{
				_tabs.Children.Add(FavoritesPage);
			}
			else if(_favoritesPage != null)
			{
				_tabs.Children.Remove(_favoritesPage);
				_favoritesPage = null;
			}
		}
	}
}



using System;

using Xamarin.Forms;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		#region Private fields

		private NavigationPage _productsPage;
		private ProductsPage productsPage;
		private NavigationPage ProductsPage
		{
			get
			{
				if (_productsPage == null)
				{
					_productsPage = new NavigationPage(productsPage = new ProductsPage() { Title = AppResources.TitleProducts });
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
					_storesPage = new NavigationPage(new StoresPage() { Title = AppResources.TitleStores });
				}
				return _storesPage;
			}
		}

		#endregion

		#region Constructors

		public RootPage()
		{						
			this.Master = new SideMenuPage()
			{
				Title = AppResources.AppName
			};
						
			this.Detail = ProductsPage;
		}

		#endregion

		#region Public API

		public void SelectProductsPage()
		{
			this.Detail = this.ProductsPage;
		}

		public void SelectStoresPage()
		{
			this.Detail = this.StoresPage;
		}

		#endregion
	}
}



using Tojeero.Core.Resources;
using Tojeero.Forms.Views.Product;
using Tojeero.Forms.Views.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
{
	public class TopTabPage : MultiPage<Page>
	{
		#region Private fields

		private ProductsPage _productsPage;
		private ProductsPage ProductsPage
		{
			get
			{
				if (_productsPage == null)
				{
					_productsPage = new ProductsPage() { Title = AppResources.TitleProducts };
					this.Children.Add(_productsPage);
				}
				return _productsPage;
			}
		}

		private StoresPage _storesPage;
		private StoresPage StoresPage
		{
			get
			{
				if (_storesPage == null)
				{
					_storesPage = new StoresPage() { Title = AppResources.TitleStores };
					this.Children.Add(_storesPage);
				}
				return _storesPage;
			}
		}

		#endregion

		#region Constructors

		public TopTabPage()
		{
			this.CurrentPage = this.ProductsPage;
		}

		#endregion

		#region Public API

		public void SelectProductsPage()
		{
			this.CurrentPage = this.ProductsPage;
		}

		public void SelectStoresPage()
		{
			this.CurrentPage = this.StoresPage;
		}

		#endregion
			
		#region implemented abstract members of MultiPage

		protected override Page CreateDefault(object item)
		{
			return this.ProductsPage;
		}

		#endregion
	}
}


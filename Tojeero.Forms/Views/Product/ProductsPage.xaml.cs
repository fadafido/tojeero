using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Product
{
	public partial class ProductsPage : BaseSearchableTabPage
	{
		#region Properties

		public new ProductsViewModel ViewModel
		{
			get
			{
				return base.ViewModel as ProductsViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Constructors

		public ProductsPage()
			: base()
		{
			InitializeComponent();

			this.ProductsButton.IsSelected = true;
			this.StoresButton.IsSelected = false;

			this.ViewModel = MvxToolbox.LoadViewModel<ProductsViewModel>();
			this.ViewModel.ShowFiltersAction = async () =>
			{
					await this.Navigation.PushModalAsync(new NavigationPage(new FilterProductsPage(this.ViewModel.SearchQuery)));
			};

			this.ViewModel.ChangeListModeAction = (mode) =>
			{
				updateListLayout(mode);
			};
			this.SearchBar.Placeholder = AppResources.PlaceholderSearchProducts;
			ListView.ItemSelected += itemSelected;
			updateListLayout(this.ViewModel.ListMode);

		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion

		#region UI Events

		private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = ((ListView)sender).SelectedItem as ProductViewModel; 
			if (item != null)
			{
				((ListView)sender).SelectedItem = null;
				var productDetails = new ProductDetailsPage(item.Product);
				await this.Navigation.PushAsync(productDetails);
			}
		}

		#endregion

		#region Utility methods

		private void updateListLayout(ListMode mode)
		{
			this.ListView.RowHeight = mode == ListMode.Normal ? 100 : 350;
			var type = mode == ListMode.Normal ? typeof(ProductListCell) : typeof(ProductListLargeCell);
			this.ListView.ItemTemplate = new DataTemplate(type);
		}

		#endregion
	}
}


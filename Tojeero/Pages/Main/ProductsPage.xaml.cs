using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;
using Tojeero.Core;

namespace Tojeero.Forms
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
			this.ToolbarItems.Add(new ToolbarItem("", "filterIcon.png", async () =>
					{
						await this.Navigation.PushModalAsync(new NavigationPage(new FilterProductsPage()));
					}));
			this.SearchBar.Placeholder = AppResources.PlaceholderSearchProducts;
			ListView.ItemSelected += itemSelected;
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
	}
}


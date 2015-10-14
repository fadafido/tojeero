using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class ProductsPage : BaseSearchablePage
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
			this.ViewModel = MvxToolbox.LoadViewModel<ProductsViewModel>();
			this.ToolbarItems.Add(new ToolbarItem("Filter", "", async () =>
					{
						await this.Navigation.PushModalAsync(new NavigationPage(new TagsPage()));
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

		private void itemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}

		#endregion
	}
}


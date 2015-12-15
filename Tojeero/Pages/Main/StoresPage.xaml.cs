using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class StoresPage : BaseSearchablePage
	{
		#region Properties

		public new StoresViewModel ViewModel
		{
			get
			{
				return base.ViewModel as StoresViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Constructors

		public StoresPage()
			: base()
		{
			InitializeComponent();
			this.ViewModel = MvxToolbox.LoadViewModel<StoresViewModel>();
			this.ToolbarItems.Add(new ToolbarItem("Filter", "", async () =>
				{
					await this.Navigation.PushModalAsync(new NavigationPage(new FilterStoresPage()));
				}));
			this.SearchBar.Placeholder = AppResources.PlaceholderSearchStores;
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

		private async void itemSelected (object sender, SelectedItemChangedEventArgs e)
		{
			var item = ((ListView)sender).SelectedItem as StoreViewModel; 
			if (item != null)
			{
				((ListView)sender).SelectedItem = null;
				var storeInfo = new StoreInfoPage(item.Store);
				await this.Navigation.PushAsync(storeInfo);
			}
		}

		#endregion
	}
}


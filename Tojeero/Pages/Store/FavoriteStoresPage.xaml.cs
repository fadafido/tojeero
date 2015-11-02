using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class FavoriteStoresPage : BaseCollectionPage
	{
		#region Constructors

		public FavoriteStoresPage()
			: base()
		{
			InitializeComponent();
			this.ViewModel = MvxToolbox.LoadViewModel<FavoriteStoresViewModel>();
			ListView.ItemSelected += itemSelected;
		}

		#endregion

		#region Properties

		public new FavoriteStoresViewModel ViewModel
		{
			get
			{
				return base.ViewModel as FavoriteStoresViewModel;
			}
			set
			{
				base.ViewModel = value;
			}
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion

		#region UI Events

		private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = ((ListView)sender).SelectedItem as StoreViewModel; 
			if (item != null)
			{
				((ListView)sender).SelectedItem = null;
			}
		}

		#endregion
	}
}


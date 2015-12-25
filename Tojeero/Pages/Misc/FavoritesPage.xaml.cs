using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Forms.Resources;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class FavoritesPage : ContentPage
	{
		#region Constructors

		public FavoritesPage()
		{			
			this.ViewModel = MvxToolbox.LoadViewModel<FavoritesViewModel>();
			InitializeComponent();
			NavigationPage.SetTitleIcon(this, "tojeero.png");
		}

		#endregion

		#region Properties

		private FavoritesViewModel _viewModel;

		public FavoritesViewModel ViewModel
		{ 
			get
			{
				return _viewModel; 
			}
			set
			{
				_viewModel = value; 
				setupViewModel();
			}
		}

		#endregion

		#region Utility methods

		private void setupViewModel()
		{
			this.ViewModel.ShowFavoriteProductsAction = async () =>
				{
					await this.Navigation.PushAsync(new FavoriteProductsPage());
				};
			this.ViewModel.ShowFavoriteStoresAction = async () =>
				{
					this.Navigation.PushAsync(new FavoriteStoresPage());
				};
			this.BindingContext = this.ViewModel;
		}

		#endregion
	}
}


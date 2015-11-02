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
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
					{
						await this.Navigation.PopModalAsync();
					}));
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
					this.Navigation.PushAsync(new FavoriteProductsPage());
				};
			this.BindingContext = this.ViewModel;
		}

		#endregion
	}
}


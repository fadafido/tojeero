using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class FilterProductsPage : ContentPage
	{
		#region Constructors

		public FilterProductsPage()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<FilterProductsViewModel>();
			InitializeComponent();
			setupPickers();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, "", async () =>
				{
					await this.Navigation.PopModalAsync();
					this.ViewModel.DoneCommand.Execute(null);
				}));
		}

		#endregion

		#region Properties

		private FilterProductsViewModel _viewModel;

		public FilterProductsViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					_viewModel = value;
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region View Lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

		#endregion

		#region Utility methods

		private void setupPickers()
		{
			citiesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
			countriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
			categoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
			subcategoriesPicker.Comparer = Comparers.UniqueEntityEqualityComparer;
		}
			
		#endregion
	}
}


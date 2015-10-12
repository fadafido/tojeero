using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class ProductsPage : ContentPage
	{
		#region Properties

		private ProductsViewModel _viewModel;
		public ProductsViewModel ViewModel
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

		#region Constructors

		public ProductsPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<ProductsViewModel>();
			this.ViewModel.ReloadFinished += reloadFinished;
			InitializeComponent();
			ProductsListView.ItemSelected += productSelected;
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
		}

		#endregion

		#region Event Handlers

		void productSelected (object sender, SelectedItemChangedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}

		void reloadFinished (object sender, EventArgs e)
		{
			this.ProductsListView.EndRefresh();
		}

		#endregion
	}
}


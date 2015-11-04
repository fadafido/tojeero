using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class ProductDetailsPage : ContentPage
	{
		
		#region Constructors

		public ProductDetailsPage(IProduct product)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<ProductDetailsViewModel>();
			this.ViewModel.Product = product;
			InitializeComponent();
			this.ViewModel.ShowStoreInfoPageAction = async (s) =>
			{
				await this.Navigation.PushAsync(new StoreInfoPage(s));
			};
		}

		#endregion

		#region Properties

		private ProductDetailsViewModel _viewModel;

		public ProductDetailsViewModel ViewModel
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
	}
}


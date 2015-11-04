﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class StoreInfoPage : ContentPage
	{
		#region Constructors

		public StoreInfoPage(IStore store)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<StoreInfoViewModel>();
			this.ViewModel.Store = store;
			InitializeComponent();
			this.HeaderView.BindingContext = this.ViewModel;
			this.listView.ItemSelected += itemSelected;
			this.ViewModel.ShowStoreDetailsAction = async (s) =>
			{
				await this.Navigation.PushAsync(new StoreDetailsPage(s));
			};
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.Products.LoadFirstPageCommand.Execute(null);
		}

		#endregion

		#region Properties

		private StoreInfoViewModel _viewModel;

		public StoreInfoViewModel ViewModel
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
					if (this.HeaderView != null)
						this.HeaderView.BindingContext = _viewModel;
					this.BindingContext = _viewModel;
				}
			}
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

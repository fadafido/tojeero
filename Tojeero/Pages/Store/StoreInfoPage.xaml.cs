﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Core;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class StoreInfoPage : ContentPage
	{
		#region Private fields and properties

		private bool _toolbarButtonsAdded = false;

		#endregion

		#region Constructors

		public StoreInfoPage(IStore store, ContentMode mode = ContentMode.View)
		{
			//Setup view model
			this.ViewModel = MvxToolbox.LoadViewModel<StoreInfoViewModel>();
			this.ViewModel.Store = store;
			this.ViewModel.Mode = mode;

			InitializeComponent();

			//Setup Header
			this.HeaderView.BindingContext = this.ViewModel;

			//Setup events
			this.listView.ItemSelected += itemSelected;
			this.ViewModel.Products.ReloadFinished += (sender, e) =>
			{
				this.listView.EndRefresh();
			};

			//Setup view model actions
			this.ViewModel.ShowStoreDetailsAction = async (s) =>
			{
				await this.Navigation.PushAsync(new StoreDetailsPage(s, mode));
			};

			this.ViewModel.AddProductAction = async (p, s) =>
			{
				await this.Navigation.PushModalAsync(new NavigationPage(new SaveProductPage(p, s)));
			};
		}

		#endregion

		#region Page lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (!_toolbarButtonsAdded)
			{
				if (this.ViewModel.Mode == ContentMode.Edit)
				{
					if (this.ViewModel.Store != null)
					{
						this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
								{
									var editStorePage = new SaveStorePage(this.ViewModel.Store);
									await this.Navigation.PushModalAsync(new NavigationPage(editStorePage));
								}));
					}
				}

				//if this view is not inside root page add close button
				var root = this.FindParent<RootPage>();
				if (root == null)
				{
					this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
							{
								await this.Navigation.PopModalAsync();
						}, priority: 15));
				}
				_toolbarButtonsAdded = true;
			}
			this.ViewModel.Products.LoadFirstPageCommand.Execute(null);
			this.ViewModel.ReloadCommand.Execute(null);
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
				var productDetails = new ProductDetailsPage(item.Product, this.ViewModel.Mode);
				await this.Navigation.PushAsync(productDetails);
			}
		}

		#endregion
	}
}


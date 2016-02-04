﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Forms.Pages.Chat;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class ProductDetailsPage : ContentPage
	{
		
		#region Constructors

		public ProductDetailsPage(IProduct product, ContentMode mode = ContentMode.View)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<ProductDetailsViewModel>();
			this.ViewModel.Product = product;
			this.ViewModel.Mode = mode;
			InitializeComponent();
			this.ViewModel.ShowStoreInfoPageAction = async (s) =>
			{
				await this.Navigation.PushAsync(new StoreInfoPage(s));
			};

			if (mode == ContentMode.Edit)
			{
				this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
					{
						var saveProductPage = new SaveProductPage(product, product.Store);
						await this.Navigation.PushModalAsync(new NavigationPage(saveProductPage));
					}));
			}
		    this.ViewModel.ShowChatPageAction = async (p, s) =>
		    {
                var channel = new ChatChannel()
                {
                    ChannelID = "test_channel",
                    RecipientID = "3UrStgR0xf",
                    RecipientProfilePictureUrl = "https://graph.facebook.com/132614200423948/picture?width=300&height=300",
                    SenderID = "WolZCU1LjR",
                    SenderProfilePictureUrl = "https://graph.facebook.com/965828503464964/picture?width=300&height=300"
                };
                var chatPage = new ChatPage(channel);
                await this.Navigation.PushAsync(chatPage);
		    };
			this.carouselLayout.IndicatorStyle = CarouselLayout.IndicatorStyleEnum.Dots;
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

		#region Parent 

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

		#endregion
	}
}


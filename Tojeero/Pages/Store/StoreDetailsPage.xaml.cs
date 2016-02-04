using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Forms.Pages.Chat;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class StoreDetailsPage : ContentPage
	{
		
		#region Constructors

		public StoreDetailsPage(IStore store, ContentMode mode = ContentMode.View)
		{
			this.ViewModel = MvxToolbox.LoadViewModel<StoreDetailsViewModel>();
			this.ViewModel.Store = store;
			this.ViewModel.Mode = mode;
			InitializeComponent();
			this.deliveryNotes.DidOpen += (sender, e) => {
				this.scrollView.ScrollToAsync(this.deliveryNotes, ScrollToPosition.End, true);
			};
			if (mode == ContentMode.Edit && store != null)
			{
				this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
					{
						var editStorePage = new SaveStorePage(store);
						await this.Navigation.PushModalAsync(new NavigationPage(editStorePage));
					}));
			}
		    this.ViewModel.ShowChatPageAction = async (s) =>
		    {
                var channel = new ChatChannel()
                {
                    ChannelID = "test_channel",
                    SenderID = "3UrStgR0xf",
                    SenderProfilePictureUrl = "https://graph.facebook.com/132614200423948/picture?width=300&height=300",
                    RecipientID = "WolZCU1LjR",
                    RecipientProfilePictureUrl = "https://graph.facebook.com/965828503464964/picture?width=300&height=300"
                };
		        var chatPage = new ChatPage(channel);
                await this.Navigation.PushAsync(chatPage);
            };
		}

		#endregion

		#region Properties

		private StoreDetailsViewModel _viewModel;

		public StoreDetailsViewModel ViewModel
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

		#region Lifecycle management

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

		#endregion
	}
}


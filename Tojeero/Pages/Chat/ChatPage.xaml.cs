using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.BL.Contracts;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.ViewModels.Chat;
using Tojeero.Forms.ViewModels.Misc;
using Xamarin.Forms;

namespace Tojeero.Forms.Pages.Chat
{
    public partial class ChatPage : ContentPage
    {
        #region Constructors
        public ChatPage(IChatChannel channel = null)
        {
            this.ViewModel = MvxToolbox.LoadViewModel<ChatChannelViewModel>();
            this.ViewModel.Channel = channel;
            this.ViewModel.ScrollToMessageAction = m =>
            {
                listView.ScrollTo(m, ScrollToPosition.MakeVisible, true);
            };
            InitializeComponent();
            listView.ItemSelected += ListViewOnItemSelected;
        }

        private async void ListViewOnItemSelected(object sender, SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            
            var item = listView.SelectedItem as ChatMessageViewModel;
            if (item?.Product?.Product != null)
            {
                listView.SelectedItem = null;
                var productDetails = new ProductDetailsPage(item.Product?.Product);
                await this.Navigation.PushAsync(productDetails);
            }
        }

        #endregion

        #region Properties
        private ChatChannelViewModel _viewModel;
        public ChatChannelViewModel ViewModel  
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if(_viewModel != value)
                {
                    _viewModel = value;
                    this.BindingContext = value;
                }
            }
        }
        #endregion
    }
}

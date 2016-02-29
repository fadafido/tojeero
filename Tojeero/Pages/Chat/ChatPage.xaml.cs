using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.ViewModels.Chat;
using Tojeero.Forms.ViewModels.Misc;
using Xamarin.Forms;

namespace Tojeero.Forms.Pages.Chat
{
    public partial class ChatPage : ContentPage
    {
        #region Constructors
        public ChatPage(IChatChannel channel = null, IProduct product = null)
        {
            this.ViewModel = MvxToolbox.LoadViewModel<ChatViewModel>();
            this.ViewModel.Channel = channel;
            this.ViewModel.ProductViewModel = new ProductViewModel(product)
            {
                FavoriteToggleEnabled = false
            };
            this.ViewModel.ScrollToMessageAction = m =>
            {
                listView.ScrollTo(m, ScrollToPosition.MakeVisible, true);
            };
            ViewModel.WillChangeMessagesCollection += ViewModelOnWillChangeMessagesCollection;
            ViewModel.DidChangeMessagesCollection += ViewModelOnDidChangeMessagesCollection;
            InitializeComponent();
            listView.ItemSelected += ListViewOnItemSelected;
            productView.ProductImage.WidthRequest = 80;
        }

        #endregion

        #region Properties
        private ChatViewModel _viewModel;
        public ChatViewModel ViewModel  
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

        #region Parent override

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.InitCommand.Execute(null);
        }

        #endregion

        #region Utility methods

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

        private void ViewModelOnWillChangeMessagesCollection(object sender, EventArgs eventArgs)
        {
            listView.SaveScrollPosition?.Invoke();
        }

        private void ViewModelOnDidChangeMessagesCollection(object sender, EventArgs eventArgs)
        {
            listView.RestoreScrollPosition?.Invoke();
        }

        #endregion
    }
}

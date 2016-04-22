using System;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Chat;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Views.Product;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Chat
{
    public partial class ChatPage : ContentPage
    {
        #region Constructors

        public ChatPage(IChatChannel channel = null, IProduct product = null)
        {
            ViewModel = MvxToolbox.LoadViewModel<ChatViewModel>();
            ViewModel.Channel = channel;
            ViewModel.ProductViewModel = new ProductViewModel(product)
            {
                FavoriteToggleEnabled = false
            };
            ViewModel.ScrollToMessageAction = m => { listView.ScrollTo(m, ScrollToPosition.MakeVisible, true); };
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
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindingContext = value;
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

        private async void ListViewOnItemSelected(object sender,
            SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            var item = listView.SelectedItem as ChatMessageViewModel;
            if (item?.Product?.Product != null)
            {
                listView.SelectedItem = null;
                var productDetails = new ProductDetailsPage(item.Product?.Product);
                await Navigation.PushAsync(productDetails);
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
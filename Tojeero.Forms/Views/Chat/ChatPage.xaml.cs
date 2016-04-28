using System;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Chat;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Forms.Views.Product;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace Tojeero.Forms.Views.Chat
{
    public partial class ChatPage
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
            ViewModel.ShowProductDetailsAction = p =>
            {
                Navigation.PushAsync(new ProductDetailsPage(p));
            };
            InitializeComponent();
            
            productView.ProductImage.WidthRequest = 80;
        }

        #endregion

        #region Utility methods

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
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
        #region Private fields

        private readonly IChatChannel _channel;
        private readonly IProduct _product;

        #endregion

        #region Constructors

        public ChatPage(IChatChannel channel = null, IProduct product = null)
        {
            InitializeComponent();

            _channel = channel;
            _product = product;

            ViewModel = MvxToolbox.LoadViewModel<ChatViewModel>();
            
            productView.ProductImage.WidthRequest = 80;
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.Channel = _channel;
            ViewModel.ProductViewModel = new ProductViewModel(_product)
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
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Views.Chat;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class StoreDetailsPage : ContentPage
    {
        #region Constructors

        public StoreDetailsPage(IStore store, ContentMode mode = ContentMode.View)
        {
            ViewModel = MvxToolbox.LoadViewModel<StoreDetailsViewModel>();
            ViewModel.Store = store;
            ViewModel.Mode = mode;
            InitializeComponent();
            deliveryNotes.DidOpen +=
                (sender, e) => { scrollView.ScrollToAsync(deliveryNotes, ScrollToPosition.End, true); };
            if (mode == ContentMode.Edit && store != null)
            {
                ToolbarItems.Add(new ToolbarItem(AppResources.ButtonEdit, "", async () =>
                {
                    var editStorePage = new SaveStorePage(store);
                    await Navigation.PushModalAsync(new NavigationPage(editStorePage));
                }));
            }
            ViewModel.ShowChatPageAction = async channel =>
            {
                var chatPage = new ChatPage(channel);
                await Navigation.PushAsync(chatPage);
            };
        }

        #endregion

        #region Properties

        private StoreDetailsViewModel _viewModel;

        public StoreDetailsViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    BindingContext = _viewModel;
                }
            }
        }

        #endregion

        #region Lifecycle management

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
        }

        #endregion
    }
}
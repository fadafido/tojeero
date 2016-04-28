using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Views.Chat;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class StoreDetailsPage
    {
        #region Private fields

        private readonly IStore _store;
        private readonly ContentMode _mode;

        #endregion


        #region Constructors

        public StoreDetailsPage(IStore store, ContentMode mode = ContentMode.View)
        {
            InitializeComponent();

            _mode = mode;
            _store = store;
            ViewModel = MvxToolbox.LoadViewModel<StoreDetailsViewModel>();

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
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.Store = _store;
            ViewModel.Mode = _mode;
            ViewModel.ShowChatPageAction = async channel =>
            {
                var chatPage = new ChatPage(channel);
                await Navigation.PushAsync(chatPage);
            };
        }

        #endregion
    }
}
using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Main;
using Tojeero.Forms.Views.Store;
using Tojeero.Forms.Views.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
{
    public partial class SideMenuPage : ContentPage
    {
        #region Properties

        private SideMenuViewModel _viewModel;

        public SideMenuViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    setupViewModel();
                }
            }
        }

        #endregion

        #region Constructors

        public SideMenuPage()
        {
            ViewModel = MvxToolbox.LoadViewModel<SideMenuViewModel>();
            InitializeComponent();
            Icon = "menuIcon.png";
        }

        #endregion

        #region View lifecycle management

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadUserStoreCommand.Execute(null);
        }

        #endregion

        #region Utility methods

        private void setupViewModel()
        {
            ViewModel.ShowProfileSettings =
                async arg => { await Navigation.PushModalAsync(new NavigationPage(new ProfileSettingsPage(arg))); };
            ViewModel.ShowLanguageChangeWarning =
                arg => { DisplayAlert(AppResources.AlertTitleWarning, arg, AppResources.OK); };

            ViewModel.ShowSaveStoreAction = async s =>
            {
                if (s == null)
                {
                    await Navigation.PushModalAsync(new NavigationPage(new SaveStorePage(s)));
                }
                else
                {
                    var storeInfoPage = new StoreInfoPage(s, ContentMode.Edit);
                    await Navigation.PushModalAsync(new NavigationPage(storeInfoPage));
                }
            };

            ViewModel.ShowTermsAction =
                async () => { await Navigation.PushModalAsync(new NavigationPage(new TermsPage())); };
            BindingContext = _viewModel;
        }

        #endregion
    }
}
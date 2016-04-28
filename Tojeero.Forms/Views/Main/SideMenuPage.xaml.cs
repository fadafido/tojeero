using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Main;
using Tojeero.Forms.Views.Store;
using Tojeero.Forms.Views.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
{
    public partial class SideMenuPage
    {

        #region Constructors

        public SideMenuPage()
        {
            InitializeComponent();

            ViewModel = MvxToolbox.LoadViewModel<SideMenuViewModel>();

            Icon = "menuIcon.png";
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
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
        }

        #endregion

    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.User
{
    public partial class ProfileSettingsPage : ContentPage
    {
        #region Properties

        private ProfileSettingsViewModel _viewModel;

        public ProfileSettingsViewModel ViewModel
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

        #region Constructors

        public ProfileSettingsPage(bool userShouldProvideDetails = false)
        {
            ViewModel =
                MvxToolbox.LoadViewModel<ProfileSettingsViewModel>(
                    new {userShouldProvideProfileDetails = userShouldProvideDetails});
            ViewModel.CloseAction = async () => { await Navigation.PopModalAsync(); };
            ViewModel.ShowTermsAction =
                async () => { await Navigation.PushModalAsync(new NavigationPage(new TermsPage())); };
            InitializeComponent();
            if (!ViewModel.UserShouldProvideProfileDetails)
            {
                ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, null,
                    () => { ViewModel.SubmitCommand.Execute(null); }));
                ToolbarItems.Add(new ToolbarItem(AppResources.ButtonCancel, null,
                    async () => { await Navigation.PopModalAsync(); }, priority: 15));
            }
            setupPickers();
        }

        #endregion

        #region View Lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ReloadCommand.Execute(null);
        }

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            countriesPicker.FacetsLoader = ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => ViewModel.Countries);

            citiesPicker.FacetsLoader = ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => ViewModel.Country?.Cities);
        }

        #endregion
    }
}
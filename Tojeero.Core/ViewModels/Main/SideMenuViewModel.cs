using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Main
{
    public class SideMenuViewModel : BaseUserStoreViewModel
    {
        #region Private fields and properties

        private readonly ILocalizationService _localizationService;
        private readonly MvxSubscriptionToken _languageChangeToken;

        #endregion

        #region Constructors

        public SideMenuViewModel(IAuthenticationService authService, IMvxMessenger messenger,
            ILocalizationService localizationService)
            : base(authService, messenger)
        {
            ShouldSubscribeToSessionChange = true;
            _localizationService = localizationService;
            _languageChangeToken =
                messenger.Subscribe<LanguageChangedMessage>(message => { RaisePropertyChanged(() => NewLanguage); });

            PropertyChanged += propertyChanged;
        }

        #endregion

        #region View lifecycle management

        public override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserStoreCommand.Execute(null);
        }

        #endregion

        #region Properties

        public Action<bool> ShowProfileSettings { get; set; }
        public Action<string> ShowLanguageChangeWarning { get; set; }
        public Action ShowTermsAction { get; set; }

        public LanguageCode NewLanguage
        {
            get
            {
                return _localizationService.Language == LanguageCode.Arabic ? LanguageCode.English : LanguageCode.Arabic;
            }
        }

        #endregion

        #region Commands

        private MvxCommand _loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                _loginCommand = _loginCommand ?? new MvxCommand(async () => await logIn(), () => CanExecuteLoginCommand);
                return _loginCommand;
            }
        }

        private MvxCommand _logoutCommand;

        public ICommand LogoutCommand
        {
            get
            {
                _logoutCommand = _logoutCommand ?? new MvxCommand(async () => await logOut(), () => !IsLoading);
                return _logoutCommand;
            }
        }

        public bool CanExecuteLoginCommand
        {
            get { return !IsLoading && IsNetworkAvailable; }
        }

        private MvxCommand _showProfileSettingsCommand;

        public ICommand ShowProfileSettingsCommand
        {
            get
            {
                _showProfileSettingsCommand = _showProfileSettingsCommand ??
                                              new MvxCommand(() => { ShowProfileSettings.Fire(false); });
                return _showProfileSettingsCommand;
            }
        }

        private MvxCommand _changeLanguageCommand;

        public ICommand ChangeLanguageCommand
        {
            get
            {
                _changeLanguageCommand = _changeLanguageCommand ?? new MvxCommand(() =>
                {
                    Mvx.Resolve<ILocalizationService>().SetLanguage(NewLanguage);
                    ShowLanguageChangeWarning.Fire(AppResources.MessageLanguageChangeWarning);
                });
                return _changeLanguageCommand;
            }
        }

        private MvxCommand _showTermsCommand;

        public ICommand ShowTermsCommand
        {
            get
            {
                _showTermsCommand = _showTermsCommand ?? new MvxCommand(() => { ShowTermsAction.Fire(); });
                return _showTermsCommand;
            }
        }

        #endregion

        #region Utility Methods

        private async Task logIn()
        {
            IsLoading = true;
            var result = await _authService.LogInWithFacebook();
            if (result != null && !result.IsProfileSubmitted)
            {
                ShowProfileSettings.Fire(true);
            }
            IsLoading = false;
        }

        private async Task logOut()
        {
            IsLoading = true;
            await _authService.LogOut();
            IsLoading = false;
        }

        void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoadingProperty || e.PropertyName == IsNetworkAvailableProperty)
            {
                RaisePropertyChanged(() => CanExecuteLoginCommand);
            }
        }

        #endregion
    }
}
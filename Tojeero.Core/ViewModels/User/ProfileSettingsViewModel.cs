using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Connectivity.Plugin.Abstractions;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.User
{
    public class ProfileSettingsViewModel : BaseUserViewModel
    {
        #region Private Fields and Properties

        private enum Commands
        {
            Unknown,
            LoadCountries,
            LoadCities,
            Submit
        }

        CancellationTokenSource _tokenSource;
        CancellationToken _token;
        private Commands _lastExecutedCommand;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;
        private readonly IUserManager _userManager;
        Dictionary<int, ICity[]> _citiesCache;

        #endregion

        #region Constructors

        public ProfileSettingsViewModel(IAuthenticationService authService, IMvxMessenger messenger,
            ICountryManager countryManager, ICityManager cityManager,
            IUserManager userManager)
            : base(authService, messenger)
        {
            ShouldSubscribeToSessionChange = true;
            _countryManager = countryManager;
            _cityManager = cityManager;
            _userManager = userManager;
            PropertyChanged += propertyChanged;
            updateUserData();
        }

        public void Init(bool userShouldProvideProfileDetails)
        {
            UserShouldProvideProfileDetails = userShouldProvideProfileDetails;
            Hint = UserShouldProvideProfileDetails ? AppResources.MessageProfileSettingsHint : null;
        }

        #endregion

        #region Properties

        public Action CloseAction { get; set; }
        public Action ShowTermsAction { get; set; }

        private string _hint;

        public string Hint
        {
            get { return _hint; }
            set
            {
                _hint = value;
                RaisePropertyChanged(() => Hint);
            }
        }

        private bool _userShouldProvideProfileDetails;

        public bool UserShouldProvideProfileDetails
        {
            get { return _userShouldProvideProfileDetails; }
            private set
            {
                _userShouldProvideProfileDetails = value;
                RaisePropertyChanged(() => UserShouldProvideProfileDetails);
            }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged(() => FirstName);
                RaisePropertyChanged(() => FullName);
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged(() => LastName);
                RaisePropertyChanged(() => FullName);
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        private string _profilePicture;

        public string ProfilePicture
        {
            get { return _profilePicture; }
            set
            {
                _profilePicture = value;
                RaisePropertyChanged(() => ProfilePicture);
            }
        }


        private ICountry _country;

        public ICountry Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    RaisePropertyChanged(() => Country);
                }
            }
        }

        private ICity _city;

        public ICity City
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    _city = value;
                    RaisePropertyChanged(() => City);
                }
            }
        }

        private string _mobile;

        public string Mobile
        {
            get { return _mobile; }
            set
            {
                _mobile = value;
                RaisePropertyChanged(() => Mobile);
            }
        }

        private ICountry[] _countries;

        public ICountry[] Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
                RaisePropertyChanged(() => Countries);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCountryFacets
        {
            get { return () => _countryManager.GetProductCountryFacets("", null); }
        }

        public Func<Task<Dictionary<string, int>>> FetchCityFacets
        {
            get { return () => _cityManager.GetProductCityFacets("", null); }
        }

        #endregion

        #region Commands

        private MvxCommand _tryAgainCommand;

        public ICommand TryAgainCommand
        {
            get
            {
                _tryAgainCommand = _tryAgainCommand ?? new MvxCommand(tryAgain, () => !IsLoading);
                return _tryAgainCommand;
            }
        }

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ??
                                 new MvxCommand(async () => { await reloadCountries(); },
                                     () => (Countries == null || Countries.Length == 0) && !IsLoading);
                return _reloadCommand;
            }
        }

        private MvxCommand _submitCommand;

        public ICommand SubmitCommand
        {
            get
            {
                _submitCommand = _submitCommand ?? new MvxCommand(async () =>
                {
                    _lastExecutedCommand = Commands.Submit;
                    await submit();
                }, () => CanExecuteSubmitCommand);
                return _submitCommand;
            }
        }

        public bool CanExecuteSubmitCommand
        {
            get { return !IsLoading && IsNetworkAvailable; }
        }

        private MvxCommand _cancelCommand;

        public ICommand CancelCommand
        {
            get
            {
                _cancelCommand = _cancelCommand ?? new MvxCommand(() => cancel());
                return _cancelCommand;
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

        #region Protected API

        protected override void handleNetworkConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            base.handleNetworkConnectionChanged(sender, e);
            if (e.IsConnected)
            {
                TryAgainCommand.Execute(null);
            }
        }

        #endregion

        #region Utility Methods

        private async Task submit()
        {
            StartLoading(AppResources.MessageSubmitting);
            using (_tokenSource = new CancellationTokenSource(Constants.DefaultTimeout))
            {
                _token = _tokenSource.Token;
                try
                {
                    var user = getUpdatedUser();
                    await _authService.UpdateUserDetails(user, _token);
                    StopLoading();
                    CloseAction.Fire();
                }
                catch (OperationCanceledException ex)
                {
                    Tools.Logger.Log(ex, LoggingLevel.Warning);
                    StopLoading(AppResources.MessageSubmissionTimeoutFailure);
                }
                catch (Exception ex)
                {
                    Tools.Logger.Log("Error occured while saving user details. {0}", ex.ToString(), LoggingLevel.Error);
                    StopLoading(AppResources.MessageSubmissionUnknownFailure);
                }
            }
            _tokenSource = null;
        }

        private void cancel()
        {
            //If we have a token source it mean that submit process has already started
            //so we need to cancel it and the view will be closed inside submit method
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
            //Otherwise just close the view
            else
            {
                CloseAction.Fire();
            }
        }

        private IUser getUpdatedUser()
        {
            var user = _userManager.Create();
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.CountryId = Country != null ? Country.ID : null;
            user.CityId = City != null ? City.ID : null;
            user.Mobile = Mobile;
            return user;
        }


        private async void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == CurrentUserProperty)
            {
                updateUserData();
            }
            else if (e.PropertyName == "Countries")
            {
                updateCountrySelection();
            }
            else if (e.PropertyName == "Country")
            {
                await updateCitySelection();
            }
            else if (e.PropertyName == IsLoadingProperty || e.PropertyName == IsNetworkAvailableProperty)
            {
                RaisePropertyChanged(() => CanExecuteSubmitCommand);
            }
        }

        private async Task reloadCountries()
        {
            _lastExecutedCommand = Commands.LoadCountries;
            StartLoading(AppResources.MessageGeneralLoading);
            try
            {
                var countries = await _countryManager.Fetch();
                Countries = countries.OrderBy(c => c.Name).ToArray();
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                StopLoading(AppResources.MessageLoadingTimeOut);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading countries in ProfileSettings screen.",
                    LoggingLevel.Error, true);
                StopLoading(AppResources.MessageLoadingFailed);
            }
            IsLoading = false;
        }

        private async Task reloadCities()
        {
            _lastExecutedCommand = Commands.LoadCities;
            if (Country == null)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            try
            {
                if (Country.Cities == null)
                    await Country.LoadCities();
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                StopLoading(AppResources.MessageLoadingTimeOut);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occured while loading cities in ProfileSettings screen.", LoggingLevel.Error,
                    true);
                StopLoading(AppResources.MessageLoadingFailed);
            }
            IsLoading = false;
        }

        private async void tryAgain()
        {
            //Try again only if previously something went wrong,
            //that is LoadingFailureMessage is not empty
            if (string.IsNullOrEmpty(LoadingFailureMessage))
                return;
            switch (_lastExecutedCommand)
            {
                case Commands.LoadCountries:
                    ReloadCommand.Execute(null);
                    break;
                case Commands.LoadCities:
                    await reloadCities();
                    break;
                case Commands.Submit:
                    SubmitCommand.Execute(null);
                    break;
                default:
                    break;
            }
        }

        void updateUserData()
        {
            var user = CurrentUser ?? _userManager.Create();
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            ProfilePicture = user.ProfilePictureUrl;
            Mobile = user.Mobile;
            updateCountrySelection();
        }

        void updateCountrySelection()
        {
            if (Countries != null && Settings.CountryId != null)
            {
                var country = Countries.Where(c => c.ID == Settings.CountryId).FirstOrDefault();
                Country = country;
            }
        }

        private async Task updateCitySelection()
        {
            if (Country == null)
            {
                City = null;
            }
            else
            {
                await reloadCities();
                if (Country != null && Country.Cities != null)
                {
                    var city = Country.Cities.Where(c => c.ID == Settings.CityId).FirstOrDefault();
                    City = city;
                }
                else
                {
                    City = null;
                }
            }
        }

        #endregion
    }
}
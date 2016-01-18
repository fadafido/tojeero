using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using System.Collections.Generic;
using System.Linq;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
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
			this.ShouldSubscribeToSessionChange = true;
			this._countryManager = countryManager;
			this._cityManager = cityManager;
			this._userManager = userManager;
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
			get
			{
				return _hint; 
			}
			set
			{
				_hint = value; 
				RaisePropertyChanged(() => Hint); 
			}
		}

		private bool _userShouldProvideProfileDetails;

		public bool UserShouldProvideProfileDetails
		{ 
			get
			{
				return _userShouldProvideProfileDetails; 
			}
			private set
			{
				_userShouldProvideProfileDetails = value; 
				RaisePropertyChanged(() => UserShouldProvideProfileDetails); 
			}
		}

		private string _firstName;

		public string FirstName
		{ 
			get
			{
				return _firstName; 
			}
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
			get
			{
				return _lastName; 
			}
			set
			{
				_lastName = value; 
				RaisePropertyChanged(() => LastName); 
				RaisePropertyChanged(() => FullName); 
			}
		}

		public string FullName
		{ 
			get
			{
				return string.Format("{0} {1}", FirstName, LastName); 
			}
		}

		private string _email;

		public string Email
		{ 
			get
			{
				return _email; 
			}
			set
			{
				_email = value; 
				RaisePropertyChanged(() => Email); 
			}
		}

		private string _profilePicture;

		public string ProfilePicture
		{ 
			get
			{
				return _profilePicture; 
			}
			set
			{
				_profilePicture = value; 
				RaisePropertyChanged(() => ProfilePicture); 
			}
		}


		private ICountry _country;

		public ICountry Country
		{ 
			get
			{
				return _country; 
			}
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
			get
			{
				return _city; 
			}
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
			get
			{
				return _mobile; 
			}
			set
			{
				_mobile = value; 
				RaisePropertyChanged(() => Mobile); 
			}
		}

		private ICountry[] _countries;

		public ICountry[] Countries
		{
			get
			{
				return _countries;
			}
			set
			{
				_countries = value;
				RaisePropertyChanged(() => Countries);
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _tryAgainCommand;

		public System.Windows.Input.ICommand TryAgainCommand
		{
			get
			{
				_tryAgainCommand = _tryAgainCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(tryAgain, () => !IsLoading);
				return _tryAgainCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{						
						await reloadCountries();
					}, () => (this.Countries == null || this.Countries.Length == 0) && !IsLoading);
				return _reloadCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _submitCommand;

		public System.Windows.Input.ICommand SubmitCommand
		{
			get
			{
				_submitCommand = _submitCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						_lastExecutedCommand = Commands.Submit;
						await submit();
					}, () => CanExecuteSubmitCommand);
				return _submitCommand;
			}
		}

		public bool CanExecuteSubmitCommand
		{
			get
			{
				return !IsLoading && IsNetworkAvailable;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _cancelCommand;

		public System.Windows.Input.ICommand CancelCommand
		{
			get
			{
				_cancelCommand = _cancelCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => cancel());
				return _cancelCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showTermsCommand;

		public System.Windows.Input.ICommand ShowTermsCommand
		{
			get
			{
				_showTermsCommand = _showTermsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>{
					ShowTermsAction.Fire();
				});
				return _showTermsCommand;
			}
		}

		#endregion

		#region Protected API

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			if (e.IsConnected)
			{
				this.TryAgainCommand.Execute(null);
			}
		}

		#endregion

		#region Utility Methods

		private async Task submit()
		{
			this.StartLoading(AppResources.MessageSubmitting);
			using (_tokenSource = new CancellationTokenSource(Constants.DefaultTimeout))
			{
				_token = _tokenSource.Token;
				try
				{
					var user = getUpdatedUser();
					await _authService.UpdateUserDetails(user, _token);
					this.StopLoading();
					this.CloseAction.Fire();
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
				this.CloseAction.Fire();
			}
		}

		private IUser getUpdatedUser()
		{
			var user = _userManager.Create();
			user.FirstName = this.FirstName;
			user.LastName = this.LastName;
			user.CountryId = this.Country != null ? this.Country.ID : null;
			user.CityId = this.City != null ? this.City.ID : null;
			user.Mobile = this.Mobile;
			return user;
		}


		private async void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
			this.StartLoading(AppResources.MessageGeneralLoading);
			try
			{
				var countries = await _countryManager.Fetch();
				this.Countries = countries.OrderBy(c => c.Name).ToArray();
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading(AppResources.MessageLoadingTimeOut);
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while loading countries in ProfileSettings screen.", LoggingLevel.Error, true);
				StopLoading(AppResources.MessageLoadingFailed);
			}
			this.IsLoading = false;
		}

		private async Task reloadCities()
		{
			_lastExecutedCommand = Commands.LoadCities;
			if (this.Country == null)
				return;
			this.StartLoading(AppResources.MessageGeneralLoading);
			try
			{
				if(this.Country.Cities == null)
					await this.Country.LoadCities();
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading(AppResources.MessageLoadingTimeOut);
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while loading cities in ProfileSettings screen.", LoggingLevel.Error, true);
				StopLoading(AppResources.MessageLoadingFailed);
			}
			this.IsLoading = false;
		}

		private async void tryAgain()
		{
			//Try again only if previously something went wrong,
			//that is LoadingFailureMessage is not empty
			if (string.IsNullOrEmpty(this.LoadingFailureMessage))
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
			var user = this.CurrentUser ?? _userManager.Create();
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.ProfilePicture = user.ProfilePictureUrl;
			this.Mobile = user.Mobile;
			updateCountrySelection();
		}

		void updateCountrySelection()
		{
			if (Countries != null && Settings.CountryId != null)
			{
				var country = this.Countries.Where(c => c.ID == Settings.CountryId).FirstOrDefault();
				this.Country = country;
			}
		}

		private async Task updateCitySelection()
		{
			if (this.Country == null)
			{
				this.City = null;
			}
			else
			{	
				await reloadCities();
				if (this.Country != null && this.Country.Cities != null)
				{
					var city = this.Country.Cities.Where(c => c.ID == Settings.CityId).FirstOrDefault();
					this.City = city;
				}
				else
				{
					this.City = null;
				}
			}
		}
		#endregion

	}
}


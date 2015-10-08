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
		Dictionary<int, ICity[]> _citiesCache;

		#endregion

		#region Constructors

		public ProfileSettingsViewModel(IAuthenticationService authService, IMvxMessenger messenger, ICountryManager countryManager, ICityManager cityManager)
			: base(authService, messenger)
		{
			this._countryManager = countryManager;
			this._cityManager = cityManager;
			PropertyChanged += propertyChanged;
			updateUserData();
		}

		public void Init(bool userShouldProvideProfileDetails)
		{
			Hint = userShouldProvideProfileDetails ? AppResources.MessageProfileSettingsHint : null;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> Close;

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


		public ICountry Country
		{ 
			get
			{
				return getCurrentCountry();
			}
		}

		public ICity City
		{ 
			get
			{
				return getCurrentCity();
			}
		}

		private int _countryIndex;

		public int CountryIndex
		{ 
			get
			{
				return _countryIndex; 
			}
			set
			{
				bool isNew = _countryIndex != value;
				_countryIndex = value; 
				RaisePropertyChanged(() => CountryIndex); 
				RaisePropertyChanged(() => Country);
				if (isNew || this.Cities == null)
				{
					this.Cities = null;
					ReloadCitiesCommand.Execute(null);
				}
			}
		}

		private int _cityIndex;

		public int CityIndex
		{ 
			get
			{
				return _cityIndex; 
			}
			set
			{
				_cityIndex = value; 
				RaisePropertyChanged(() => CityIndex); 
				RaisePropertyChanged(() => City); 
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
				updateCountrySelection();
			}
		}

		private ICity[] _cities;

		public ICity[] Cities
		{ 
			get
			{
				return _cities; 
			}
			set
			{
				_cities = value; 
				RaisePropertyChanged(() => Cities); 
				updateCitySelection();
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

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCountriesCommand;

		public System.Windows.Input.ICommand ReloadCountriesCommand
		{
			get
			{
				_reloadCountriesCommand = _reloadCountriesCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						_lastExecutedCommand = Commands.LoadCountries;
						await reloadCountries();
					}, () => (this.Countries == null || this.Countries.Length == 0) && !IsLoading);
				return _reloadCountriesCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCitiesCommand;

		public System.Windows.Input.ICommand ReloadCitiesCommand
		{
			get
			{
				_reloadCitiesCommand = _reloadCitiesCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						_lastExecutedCommand = Commands.LoadCities;
						await reloadCities();
					}, () => !IsLoading && this.Country != null && this.Cities == null);
				return _reloadCitiesCommand;
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
			using (_tokenSource = new CancellationTokenSource())
			{
				_token = _tokenSource.Token;
				try
				{
					var user = getUpdatedUser();
					await _authService.UpdateUserDetails(user, _token);
					this.StopLoading();
					this.Close.Fire(this, new EventArgs());
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
				this.Close.Fire(this, new EventArgs());
			}
		}

		private User getUpdatedUser()
		{
			var user = new User();
			user.FirstName = this.FirstName;
			user.LastName = this.LastName;
			var country = getCurrentCountry();
			user.CountryId = country != null ? (int?)country.CountryId : null;
			var city = getCurrentCity();
			user.CityId = city != null ? (int?)city.CityId : null;
			user.Mobile = this.Mobile;
			return user;
		}


		void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentUser")
			{
				updateUserData();
			}
			else if (e.PropertyName == "Country")
			{
				
			}
			else if (e.PropertyName == IsLoadingPropertyName || e.PropertyName == IsNetworkAvailablePropertyName)
			{
				RaisePropertyChanged(() => CanExecuteSubmitCommand);
			}
		}

		private async Task reloadCountries()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
			try
			{
				var countries = await _countryManager.FetchCountries();
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
			if (this.Country == null)
				return;
			this.StartLoading(AppResources.MessageGeneralLoading);
			try
			{
				var cities = await _cityManager.FetchCities(this.Country.CountryId);
				this.Cities = cities.OrderBy(c => c.Name).ToArray();
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

		void tryAgain()
		{
			//Try again only if previously something went wrong,
			//that is LoadingFailureMessage is not empty
			if (string.IsNullOrEmpty(this.LoadingFailureMessage))
				return;
			switch (_lastExecutedCommand)
			{
				case Commands.LoadCountries:
					ReloadCountriesCommand.Execute(null);
					break;
				case Commands.LoadCities:
					ReloadCitiesCommand.Execute(null);
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
			var user = this.CurrentUser ?? new User();
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.ProfilePicture = user.ProfilePictureUrl;
			this.Mobile = user.Mobile;
			updateCountrySelection();
		}

		void updateCountrySelection()
		{
			if (Countries != null && this.CurrentUser != null && this.CurrentUser.CountryId != null)
			{
				int i = 0;
				foreach (var country in Countries)
				{
					if (country.CountryId == this.CurrentUser.CountryId)
						break;
					i++;
				}

				if (i < this.Countries.Length)
					this.CountryIndex = i;
				else
					this.CountryIndex = 0;
			}
			ReloadCitiesCommand.Execute(null);
		}

		void updateCitySelection()
		{
			if (Cities != null && this.CurrentUser != null && this.CurrentUser.CityId != null)
			{
				int i = 0;
				foreach (var city in Cities)
				{
					if (city.CityId == this.CurrentUser.CityId)
						break;
					i++;
				}

				if (i < this.Cities.Length)
					this.CityIndex = i;
				else
					this.CityIndex = 0;
			}
		}

		private ICountry getCurrentCountry()
		{
			var country = this.Countries != null && this.CountryIndex >= 0 && this.CountryIndex < this.Countries.Length ? this.Countries[this.CountryIndex] : null;
			return country;
		}

		private ICity getCurrentCity()
		{
			var city = this.Cities != null && this.CityIndex >= 0 && this.CityIndex < this.Cities.Length ? this.Cities[this.CityIndex] : null;
			return city;
		}


		#endregion

	}
}


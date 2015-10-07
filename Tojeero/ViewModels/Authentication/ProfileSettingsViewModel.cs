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
using Tojeero.Core.Resources;

namespace Tojeero.Core.ViewModels
{
	public class ProfileSettingsViewModel : BaseUserViewModel
	{
		#region Private Fields and Properties

		CancellationTokenSource _tokenSource;
		CancellationToken _token;
		private bool _isDataLoaded;
		private bool _isSubmissionFailed;
		private readonly ICountryManager _countryManager;

		#endregion

		#region Constructors

		public ProfileSettingsViewModel(IAuthenticationService authService, IMvxMessenger messenger, ICountryManager countryManager)
			: base(authService, messenger)
		{
			this._countryManager = countryManager;
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

		private int _countryIndex;
		public int CountryIndex
		{ 
			get
			{
				return _countryIndex; 
			}
			set
			{
				_countryIndex = value; 
				RaisePropertyChanged(() => CountryIndex); 
				RaisePropertyChanged(() => Country); 
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
				updateSelections();
			}
		}

		private List<string> _cities;
		public List<string> Cities
		{ 
			get
			{
				return _cities; 
			}
			private set
			{
				_cities = value; 
				RaisePropertyChanged(() => Cities); 
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _tryAgainCommand;

		public System.Windows.Input.ICommand TryAgainCommand
		{
			get
			{
				_tryAgainCommand = _tryAgainCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						if(!_isDataLoaded)
							ReloadCommand.Execute(null);
						if(_isSubmissionFailed)
							SubmitCommand.Execute(null);
						
					}, () => !IsLoading && IsNetworkAvailable);
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
						await reloadData();
					}, () => !IsLoading && IsNetworkAvailable);
				return _reloadCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _submitCommand;

		public System.Windows.Input.ICommand SubmitCommand
		{
			get
			{
				_submitCommand = _submitCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await submit(), () => CanExecuteSubmitCommand);
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
			if (e.IsConnected && !_isDataLoaded)
			{
				this.TryAgainCommand.Execute(null);
			}
		}

		#endregion

		#region Utility Methods

		private async Task submit()
		{
			this.StartLoading("Submitting data...");
			_isSubmissionFailed = false;
			using (_tokenSource = new CancellationTokenSource())
			{
				_token = _tokenSource.Token;
				try
				{
					var user = getUpdatedUser();
					await _authService.UpdateUserDetails(user, _token);
					_isSubmissionFailed = false;
					this.StopLoading();
					this.Close.Fire(this, new EventArgs());
					return;
				}
				catch(OperationCanceledException ex)
				{
					Tools.Logger.Log(ex, LoggingLevel.Warning);
					StopLoading("Submission failed because of timeout. Please try again.");
				}
				catch (Exception ex)
				{
					Tools.Logger.Log("Error occured while saving user details. {0}", ex.ToString(), LoggingLevel.Error);
					StopLoading("Submission failed because of unknown error. Please try again. If the issue persists please contact our support.");
				}
			}
			_isSubmissionFailed = true;
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
				_submitCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => CanExecuteSubmitCommand);
			}
		}

		private async Task reloadData()
		{
			if (_isDataLoaded)
				return;			
			this.StartLoading("Loading data...");
			try
			{
				var countries = await _countryManager.FetchCountries();
				this.Countries = countries.ToArray();;
				_isDataLoaded = true;
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				StopLoading("Failed to load data because of timeout. Please try again.");
			}
			catch (Exception ex)
			{
				Tools.Logger.Log("Error occured while saving user details. {0}", ex.ToString(), LoggingLevel.Error);
				StopLoading(AppResources.MessageLoadingFailed);
			}
			this.IsLoading = false;
		}

		void updateUserData()
		{
			var user = this.CurrentUser ?? new User();
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.ProfilePicture = user.ProfilePictureUrl;
			this.Mobile = user.Mobile;
			updateSelections();
		}

		void updateSelections()
		{
			if(Countries != null && this.CurrentUser != null && this.CurrentUser.CountryId != null)
			{
				int i = 0;
				foreach (var country in Countries)
				{
					if (country.CountryId == this.CurrentUser.CountryId)
						break;
					i++;
				}

				if(i < this.Countries.Length)
					this.CountryIndex = i;
			}
		}

		private ICountry getCurrentCountry()
		{
			var country = this.Countries != null && this.CountryIndex >= 0 && this.CountryIndex < this.Countries.Length ? this.Countries[this.CountryIndex] : null;
			return country;
		}
		#endregion

	}
}


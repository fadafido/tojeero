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
		private readonly ICountryManager _countryManager;

		#endregion

		#region Constructors

		public ProfileSettingsViewModel(IAuthenticationService authService, IMvxMessenger messenger, ICountryManager countryManager)
			: base(authService, messenger)
		{
			this._countryManager = countryManager;
			PropertyChanged += propertyChanged;
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

		private IEnumerable<ICountry> _countries;
		public IEnumerable<ICountry> Countries
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
			if (!_isDataLoaded)
			{
				this.ReloadCommand.Execute(null);
			}
		}

		#endregion

		#region Utility Methods

		private async Task submit()
		{
			this.StartLoading("Submitting data...");
			using (_tokenSource = new CancellationTokenSource())
			{
				_token = _tokenSource.Token;
				try
				{
					await _authService.UpdateUserDetails(this.CurrentUser, _token);
					this.StopLoading();
					this.Close.Fire(this, new EventArgs());
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
			
		void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Country")
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
			await Task.Delay(2000);
			StopLoading("Failed to load data because of timeout. Please try again.");
			return;
			try
			{
				var countries = await _countryManager.FetchCountries();
				this.Countries = countries;
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
				StopLoading("Failed to load data because of unknown error. Please try again. If the issue persists please contact our support.");
			}
			this.IsLoading = false;
		}
		#endregion

	}
}


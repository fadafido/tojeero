using System;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using System.Linq;
using Tojeero.Core.Toolbox;
using Tojeero.Core.Services;
using Tojeero.Forms.Resources;
using System.Collections.Generic;

namespace Tojeero.Core.ViewModels
{
	public class BootstrapViewModel : LoadableNetworkViewModel
	{
		#region Private fields and properties

		private enum Commands
		{
			Unknown,
			LoadCountries,
			LoadCities
		}

		private Commands _lastExecutedCommand;
		private bool _isCachedCleared;
		private readonly ICountryManager _countryManager;
		private readonly ICityManager _cityManager;
		private readonly ILocalizationService _localizationService;

		#endregion

		#region Constructors

		public BootstrapViewModel(ICountryManager countryManager, ICityManager cityManager, ILocalizationService localizationService)
			: base()
		{
			this._localizationService = localizationService;
			this._countryManager = countryManager;
			this._cityManager = cityManager;
			this.Language = localizationService.Language;
		}

		#endregion

		#region Properties

		public Action BootstrapFinished { get; set; }

		public bool IsUserPreferancesSet
		{
			get
			{
				return Core.Settings.IsUserPreferancesSet;
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
					RaisePropertyChanged(() => CanExecuteApplyCommand); 
					reloadCities();
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
					RaisePropertyChanged(() => CanExecuteApplyCommand); 
				}
			}
		}

		private LanguageCode _language;

		public LanguageCode Language
		{ 
			get
			{
				return _language; 
			}
			set
			{
				if (_language != value)
				{
					_language = value; 
					RaisePropertyChanged(() => Language); 
				}
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

        public Func<Task<Dictionary<string, int>>> FetchCountryFacets
        {
            get
            {
                return () => _countryManager.GetProductCountryFacets("", null);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCityFacets
        {
            get
            {
                return () => _cityManager.GetProductCityFacets("", null);
            }
        }

        private LanguageCode[] _languages;
		public LanguageCode[] Languages
		{
			get
			{
				if (_languages == null)
				{
					_languages = new LanguageCode[] { LanguageCode.English, LanguageCode.Arabic };
				}
				return _languages;
			}
		}

		private bool _isBootstrapCompleted;

		public bool IsBootstrapCompleted
		{ 
			get
			{
				return _isBootstrapCompleted; 
			}
			set
			{
				_isBootstrapCompleted = value; 
				RaisePropertyChanged(() => IsBootstrapCompleted); 
				RaisePropertyChanged(() => ShouldShowUsePrefs); 
			}
		}

		public bool ShouldShowUsePrefs
		{
			get
			{
				return !IsUserPreferancesSet && IsBootstrapCompleted;
			}
		}

		#endregion


		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _bootstrapCommand;

		public System.Windows.Input.ICommand BootstrapCommand
		{
			get
			{
				_bootstrapCommand = _bootstrapCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await bootstrap();
					}, () => !IsLoading);
				return _bootstrapCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _tryAgainCommand;

		public System.Windows.Input.ICommand TryAgainCommand
		{
			get
			{
				_tryAgainCommand = _tryAgainCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(tryAgain, () => !IsLoading);
				return _tryAgainCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _applyCommand;

		public System.Windows.Input.ICommand ApplyCommand
		{
			get
			{
				_applyCommand = _applyCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						apply();
					}, () => true);
				return _applyCommand;
			}
		}

		public bool CanExecuteApplyCommand
		{
			get
			{
				return this.Country != null && this.City != null;
			}
		}

		#endregion

		#region Protected 

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			if (e.IsConnected && !IsBootstrapCompleted)
				this.BootstrapCommand.Execute(null);
		}

		#endregion

		#region Private API

		private async Task bootstrap()
		{			
			this.StartLoading(AppResources.MessageGeneralLoading);
			await clearCache();
			await Mvx.Resolve<IAuthenticationService>().RestoreSavedSession();
			this.StopLoading();	
			if (!this.IsUserPreferancesSet)
			{
				if (await reloadCountries())
					IsBootstrapCompleted = true;
			}
			else
			{
				BootstrapFinished.Fire();
				IsBootstrapCompleted = true;
			}
		}


		private async Task clearCache()
		{
			if (_isCachedCleared)
				return;
			try
			{
				await Mvx.Resolve<ICacheRepository>().Initialize();
				await Mvx.Resolve<ICacheRepository>().Clear();
				_isCachedCleared = true;
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while clearing local cache before application launch.", LoggingLevel.Error, true);
			}
		}
			
		private async Task<bool> reloadCountries()
		{
			_lastExecutedCommand = Commands.LoadCountries;
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failure = null;
			try
			{
				var countries = await _countryManager.Fetch();
				this.Countries = countries.OrderBy(c => c.Name).ToArray();
				ICountry country = null;
				if(this.Countries != null)
				{
					if(Settings.CountryId != null)
						country = this.Countries.Where(c => c.ID == Settings.CountryId).FirstOrDefault();
					this.Country = country != null ? country : this.Countries.FirstOrDefault();;
				}
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				failure = AppResources.MessageLoadingTimeOut;
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while loading countries in bootstrap screen.", LoggingLevel.Error, true);
				failure = AppResources.MessageLoadingFailed;
			}
			this.StopLoading(failure);
			return failure == null;
		}

		private async Task reloadCities()
		{
			_lastExecutedCommand = Commands.LoadCities;
			if (this.Country == null)
				return;
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failure = null;
			try
			{
				if(this.Country.Cities == null)
					await this.Country.LoadCities();
				ICity city = null;
				if(this.Country.Cities != null)
				{
					if(Settings.CityId != null)
						city = this.Country.Cities.Where(c => c.ID == Settings.CityId).FirstOrDefault();
					this.City = city != null ? city : this.Country.Cities.FirstOrDefault();
				}
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				failure = AppResources.MessageLoadingTimeOut;
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while loading cities in bootstrap screen.", LoggingLevel.Error, true);
				failure = AppResources.MessageLoadingFailed;
			}
			this.StopLoading(failure);
		}

		private void apply()
		{
			Settings.CountryId = this.Country.ID;
			Settings.CityId = this.City.ID;
			_localizationService.SetLanguage(this.Language);
			Settings.IsUserPreferancesSet = true;
			this.BootstrapFinished.Fire();
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
					await reloadCountries();
					break;
				case Commands.LoadCities:
					await reloadCities();
					break;
				default:
					break;
			}
		}
		#endregion

	}
}


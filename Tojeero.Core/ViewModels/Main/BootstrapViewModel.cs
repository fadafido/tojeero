using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Connectivity.Plugin.Abstractions;
using Tojeero.Core.Contracts;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Main
{
    public class BootstrapViewModel : BaseLoadableNetworkViewModel
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

        public BootstrapViewModel(ICountryManager countryManager, ICityManager cityManager,
            ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _countryManager = countryManager;
            _cityManager = cityManager;
            Language = localizationService.Language;
        }

        #endregion

        #region Lifecycle management

        public override void OnAppearing()
        {
            base.OnAppearing();
            BootstrapCommand.Execute(null);
        }

        #endregion

        #region Properties

        public Action BootstrapFinished { get; set; }

        public bool IsUserPreferancesSet
        {
            get { return Settings.IsUserPreferancesSet; }
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
                    RaisePropertyChanged(() => CanExecuteApplyCommand);
                    reloadCities();
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
                    RaisePropertyChanged(() => CanExecuteApplyCommand);
                }
            }
        }

        private LanguageCode _language;

        public LanguageCode Language
        {
            get { return _language; }
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

        private LanguageCode[] _languages;

        public LanguageCode[] Languages
        {
            get
            {
                if (_languages == null)
                {
                    _languages = new[] {LanguageCode.English, LanguageCode.Arabic};
                }
                return _languages;
            }
        }

        private bool _isBootstrapCompleted;

        public bool IsBootstrapCompleted
        {
            get { return _isBootstrapCompleted; }
            set
            {
                _isBootstrapCompleted = value;
                RaisePropertyChanged(() => IsBootstrapCompleted);
                RaisePropertyChanged(() => ShouldShowUsePrefs);
            }
        }

        public bool ShouldShowUsePrefs
        {
            get { return !IsUserPreferancesSet && IsBootstrapCompleted; }
        }

        #endregion

        #region Commands

        private MvxCommand _bootstrapCommand;

        public ICommand BootstrapCommand
        {
            get
            {
                _bootstrapCommand = _bootstrapCommand ??
                                    new MvxCommand(async () => { await bootstrap(); },
                                        () => !IsLoading && !IsBootstrapCompleted);
                return _bootstrapCommand;
            }
        }

        private MvxCommand _tryAgainCommand;

        public ICommand TryAgainCommand
        {
            get
            {
                _tryAgainCommand = _tryAgainCommand ?? new MvxCommand(tryAgain, () => !IsLoading);
                return _tryAgainCommand;
            }
        }

        private MvxCommand _applyCommand;

        public ICommand ApplyCommand
        {
            get
            {
                _applyCommand = _applyCommand ?? new MvxCommand(() => { apply(); }, () => true);
                return _applyCommand;
            }
        }

        public bool CanExecuteApplyCommand
        {
            get { return Country != null && City != null; }
        }

        #endregion

        #region Protected 

        protected override void handleNetworkConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            base.handleNetworkConnectionChanged(sender, e);
            if (e.IsConnected && !IsBootstrapCompleted)
                BootstrapCommand.Execute(null);
        }

        #endregion

        #region Private API

        private async Task bootstrap()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            await clearCache();
            await Mvx.Resolve<IAuthenticationService>().RestoreSavedSession();
            StopLoading();
            if (!IsUserPreferancesSet)
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
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while clearing local cache before application launch.",
                    LoggingLevel.Error, true);
            }
        }

        private async Task<bool> reloadCountries()
        {
            _lastExecutedCommand = Commands.LoadCountries;
            StartLoading(AppResources.MessageGeneralLoading);
            string failure = null;
            try
            {
                var countries = await _countryManager.Fetch();
                Countries = countries.OrderBy(c => c.Name).ToArray();
                ICountry country = null;
                if (Countries != null)
                {
                    if (Settings.CountryId != null)
                        country = Countries.Where(c => c.ID == Settings.CountryId).FirstOrDefault();
                    Country = country != null ? country : Countries.FirstOrDefault();
                    ;
                }
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                failure = AppResources.MessageLoadingTimeOut;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading countries in bootstrap screen.", LoggingLevel.Error,
                    true);
                failure = AppResources.MessageLoadingFailed;
            }
            StopLoading(failure);
            return failure == null;
        }

        private async Task reloadCities()
        {
            _lastExecutedCommand = Commands.LoadCities;
            if (Country == null)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            string failure = null;
            try
            {
                if (Country.Cities == null)
                    await Country.LoadCities();
                ICity city = null;
                if (Country.Cities != null)
                {
                    if (Settings.CityId != null)
                        city = Country.Cities.Where(c => c.ID == Settings.CityId).FirstOrDefault();
                    City = city != null ? city : Country.Cities.FirstOrDefault();
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
            StopLoading(failure);
        }

        private void apply()
        {
            Settings.CountryId = Country.ID;
            Settings.CityId = City.ID;
            _localizationService.SetLanguage(Language);
            Settings.IsUserPreferancesSet = true;
            BootstrapFinished.Fire();
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
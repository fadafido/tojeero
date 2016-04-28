using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Connectivity.Plugin.Abstractions;
using Nito.AsyncEx;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Xamarin.Forms;

namespace Tojeero.Core.ViewModels.Store
{
    public class FilterStoresViewModel : BaseLoadableNetworkViewModel, IDisposable
    {
        #region Private fields

        private readonly IStoreManager _storeManager;
        private readonly IStoreCategoryManager _categoryManager;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;
        private readonly ITagManager _tagManager;
        private readonly AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public FilterStoresViewModel(IStoreManager storeManager,
            IStoreCategoryManager categoryManager,
            ICountryManager countryManager,
            ICityManager cityManager,
            ITagManager tagManager)
        {
            _storeManager = storeManager;
            _tagManager = tagManager;
            _cityManager = cityManager;
            _countryManager = countryManager;
            _categoryManager = categoryManager;
            StoreFilter.PropertyChanged += StoreFilter_PropertyChanged;
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                StoreFilter.PropertyChanged -= StoreFilter_PropertyChanged;
            }
        }

        #endregion

        #region Properties

        private IStoreFilter _storeFilter;

        public IStoreFilter StoreFilter
        {
            get
            {
                if (_storeFilter == null)
                {
                    _storeFilter = RuntimeSettings.StoreFilter.Clone();
                }
                return _storeFilter;
            }
        }

        private string _query;

        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                RaisePropertyChanged(() => Query);
            }
        }

        private int _count = -1;

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged(() => Count);
                RaisePropertyChanged(() => CountLabel);
            }
        }

        private string _loadingCountLabel;

        public string CountLabel
        {
            get
            {
                RaisePropertyChanged(() => CountLabelBackgroundColor);
                RaisePropertyChanged(() => CountLabelTextColor);

                if (!string.IsNullOrEmpty(_loadingCountLabel))
                    return _loadingCountLabel;
                if (Count < 0)
                    return "";
                if (Count == 0)
                    return AppResources.MessageNoMatchingStores;
                if (Count == 1)
                    return AppResources.MessageSingleMatchingStore;
                return string.Format(AppResources.MessageMatchingStores, ((decimal) Count).GetShortString());
            }
        }

        public Color CountLabelBackgroundColor
        {
            get
            {
                if (!string.IsNullOrEmpty(_loadingCountLabel))
                    return Colors.HeaderPositive;
                if (Count < 0)
                    return Color.Transparent;
                if (Count == 0)
                    return Colors.HeaderWarning;
                return Colors.HeaderPositive;
            }
        }

        public Color CountLabelTextColor
        {
            get
            {
                if (!string.IsNullOrEmpty(_loadingCountLabel))
                    return Colors.HeaderPositiveText;
                if (Count < 0)
                    return Color.Transparent;
                if (Count == 0)
                    return Colors.HeaderWarningText;
                return Colors.HeaderPositiveText;
            }
        }

        private IStoreCategory[] _categories;

        public IStoreCategory[] Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCategoryFacets
        {
            get { return () => _categoryManager.GetFacets(Query, StoreFilter); }
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
            get { return () => _countryManager.GetStoreCountryFacets(Query, StoreFilter); }
        }


        private ICity[] _cities;

        public ICity[] Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                RaisePropertyChanged(() => Cities);
                RaisePropertyChanged(() => IsCitiesPickerEnabled);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCityFacets
        {
            get { return () => _cityManager.GetStoreCityFacets(Query, StoreFilter); }
        }

        public bool IsCitiesPickerEnabled => StoreFilter.Country != null && Cities != null && Cities.Length > 0;

        private ObservableCollection<ITag> _tags;

        public ObservableCollection<ITag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        #endregion

        #region Commands

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ?? new MvxCommand(async () => { await reload(); }, () => !IsLoading);
                return _reloadCommand;
            }
        }

        private MvxCommand _doneCommand;

        public ICommand DoneCommand
        {
            get
            {
                _doneCommand = _doneCommand ?? new MvxCommand(() => { RuntimeSettings.StoreFilter = StoreFilter; });
                return _doneCommand;
            }
        }

        private MvxCommand _clearCountryCommand;

        public ICommand ClearCountryCommand
        {
            get
            {
                _clearCountryCommand = _clearCountryCommand ?? new MvxCommand(async () =>
                {
                    StoreFilter.Country = null;
                    await ReloadCount();
                });
                return _clearCountryCommand;
            }
        }


        private MvxCommand _clearCityCommand;

        public ICommand ClearCityCommand
        {
            get
            {
                _clearCityCommand = _clearCityCommand ?? new MvxCommand(async () =>
                {
                    StoreFilter.City = null;
                    await ReloadCount();
                });
                return _clearCityCommand;
            }
        }

        private MvxCommand _clearCategoryCommand;

        public ICommand ClearCategoryCommand
        {
            get
            {
                _clearCategoryCommand = _clearCategoryCommand ?? new MvxCommand(async () =>
                {
                    StoreFilter.Category = null;
                    await ReloadCount();
                });
                return _clearCategoryCommand;
            }
        }

        private MvxCommand _resetFiltersCommand;

        public ICommand ResetFiltersCommand
        {
            get
            {
                _resetFiltersCommand = _resetFiltersCommand ?? new MvxCommand(async () =>
                {
                    StoreFilter.Country = null;
                    StoreFilter.City = null;
                    StoreFilter.Category = null;
                    StoreFilter.Tags.Clear();
                    await ReloadCount();
                });
                return _resetFiltersCommand;
            }
        }

        public async Task ReloadCount()
        {
            _loadingCountLabel = AppResources.MessageLoadingMatches;
            RaisePropertyChanged(() => CountLabel);
            try
            {
                Count = await _storeManager.Count(Query, StoreFilter);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading matching products count", LoggingLevel.Error, true);
                Count = -1;
            }
            _loadingCountLabel = null;
            RaisePropertyChanged(() => CountLabel);
        }

        #endregion

        #region Protected API

        protected override void handleNetworkConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            base.handleNetworkConnectionChanged(sender, e);
            if (e.IsConnected)
            {
                ReloadCommand.Execute(null);
            }
        }

        #endregion

        #region Utility methods

        private async Task reload()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            try
            {
                await loadCountries();
                await reloadCities();
                await loadCategories();
            }
            catch (Exception ex)
            {
                failureMessage = handleException(ex);
            }
            StopLoading(failureMessage);
        }

        private async Task reloadCities()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            using (var writerLock = await _citiesLock.WriterLockAsync())
            {
                if (StoreFilter.Country == null)
                {
                    Cities = null;
                    StoreFilter.City = null;
                }
                if (!(StoreFilter.Country == null || Countries == null || Countries.Length == 0) &&
                    !(Cities != null && Cities.Length > 0 && Cities[0].CountryId == StoreFilter.Country.ID))
                {
                    try
                    {
                        var result = await _cityManager.Fetch(StoreFilter.Country.ID);
                        Cities = result != null ? result.ToArray() : null;
                    }
                    catch (Exception ex)
                    {
                        failureMessage = handleException(ex);
                    }
                }
                //If the city name is empty it means it has not been loaded from backend
                //We need to update the product filter to include newly loaded data, 
                //so the user will see the city name
                var cityID = StoreFilter.City?.ID;
                if (string.IsNullOrEmpty(StoreFilter.City?.Name) &&
                    !string.IsNullOrEmpty(cityID) && Cities != null)
                {
                    var city = Cities.FirstOrDefault(c => c.ID == cityID);
                    if (city != null)
                        StoreFilter.City = city;
                }
            }
            StopLoading(failureMessage);
        }

        private async Task loadCategories()
        {
            if (Categories != null && Categories.Length > 0)
                return;
            var result = await _categoryManager.Fetch();
            Categories = result != null ? result.ToArray() : null;
        }

        private async Task loadCountries()
        {
            if (Countries != null && Countries.Length > 0)
                return;
            var result = await _countryManager.Fetch();
            Countries = result != null ? result.ToArray() : null;
            //If the country name is empty it means it has not been loaded from backend
            //We need to update the product filter to include newly loaded data, 
            //so the user will see the country name
            var countryID = StoreFilter.Country?.ID;
            if (string.IsNullOrEmpty(StoreFilter.Country?.Name) &&
                !string.IsNullOrEmpty(countryID) && Countries != null)
            {
                var country = Countries.FirstOrDefault(c => c.ID == countryID);
                if (country != null)
                    StoreFilter.Country = country;
            }
        }

        private async void StoreFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
            {
                RaisePropertyChanged(() => IsCitiesPickerEnabled);
                await reloadCities();
            }
            await ReloadCount();
        }

        private string handleException(Exception exception)
        {
            try
            {
                throw exception;
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                return AppResources.MessageLoadingTimeOut;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occured while loading data in store filter screen.", LoggingLevel.Error,
                    true);
                return AppResources.MessageLoadingFailed;
            }
        }

        #endregion
    }
}
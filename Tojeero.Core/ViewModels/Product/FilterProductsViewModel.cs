using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace Tojeero.Core.ViewModels.Product
{
    public class FilterProductsViewModel : BaseLoadableNetworkViewModel, IDisposable
    {
        #region Private fields

        private readonly IProductManager _productManager;
        private readonly IProductCategoryManager _categoryManager;
        private readonly IProductSubcategoryManager _subcategoryManager;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;
        private readonly ITagManager _tagManager;
        private readonly AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _subcategoriesLock = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public FilterProductsViewModel(IProductManager productManager,
            IProductCategoryManager categoryManager,
            IProductSubcategoryManager subcategoryManager,
            ICountryManager countryManager,
            ICityManager cityManager,
            ITagManager tagManager)
        {
            _productManager = productManager;
            _tagManager = tagManager;
            _cityManager = cityManager;
            _countryManager = countryManager;
            _subcategoryManager = subcategoryManager;
            _categoryManager = categoryManager;
            ProductFilter.PropertyChanged += ProductFilter_PropertyChanged;
            ProductFilter.Tags.CollectionChanged += tagsChanged;
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
                ProductFilter.PropertyChanged -= ProductFilter_PropertyChanged;
                ProductFilter.Tags.CollectionChanged -= tagsChanged;
            }
        }

        #endregion

        #region Properties

        private IProductFilter _productFilter;

        public IProductFilter ProductFilter
        {
            get
            {
                if (_productFilter == null)
                {
                    _productFilter = RuntimeSettings.ProductFilter.Clone();
                }
                return _productFilter;
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
                    return AppResources.MessageNoMatchingProducts;
                if (Count == 1)
                    return AppResources.MessageSingleMatchingProduct;
                return string.Format(AppResources.MessageMatchingProducts, ((decimal) Count).GetShortString());
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

        private IProductCategory[] _categories;

        public IProductCategory[] Categories
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
            get { return () => _categoryManager.GetFacets(Query, ProductFilter); }
        }

        private IProductSubcategory[] _subcategories;

        public IProductSubcategory[] Subcategories
        {
            get { return _subcategories; }
            set
            {
                _subcategories = value;
                RaisePropertyChanged(() => Subcategories);
                RaisePropertyChanged(() => IsSubcategoriesPickerEnabled);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchSubcategoryFacets
        {
            get { return () => _subcategoryManager.GetFacets(Query, ProductFilter); }
        }

        public bool IsSubcategoriesPickerEnabled
            => ProductFilter.Category != null && Subcategories != null && Subcategories.Length > 0;

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
            get { return () => _countryManager.GetProductCountryFacets(Query, ProductFilter); }
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
            get { return () => _cityManager.GetProductCityFacets(Query, ProductFilter); }
        }

        public bool IsCitiesPickerEnabled => ProductFilter.Country != null && Cities != null && Cities.Length > 0;

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
                _doneCommand = _doneCommand ?? new MvxCommand(() => { RuntimeSettings.ProductFilter = ProductFilter; });
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
                    ProductFilter.Country = null;
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
                    ProductFilter.City = null;
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
                    ProductFilter.Category = null;
                    await ReloadCount();
                });
                return _clearCategoryCommand;
            }
        }

        private MvxCommand _clearSubcategoryCommand;

        public ICommand ClearSubcategoryCommand
        {
            get
            {
                _clearSubcategoryCommand = _clearSubcategoryCommand ?? new MvxCommand(async () =>
                {
                    ProductFilter.Subcategory = null;
                    await ReloadCount();
                });
                return _clearSubcategoryCommand;
            }
        }

        private MvxCommand _clearStartPriceCommand;

        public ICommand ClearStartPriceCommand
        {
            get
            {
                _clearStartPriceCommand = _clearStartPriceCommand ?? new MvxCommand(async () =>
                {
                    ProductFilter.StartPrice = null;
                    await ReloadCount();
                }, () => true);
                return _clearStartPriceCommand;
            }
        }

        private MvxCommand _clearEndPriceCommand;

        public ICommand ClearEndPriceCommand
        {
            get
            {
                _clearEndPriceCommand = _clearEndPriceCommand ?? new MvxCommand(async () =>
                {
                    ProductFilter.EndPrice = null;
                    await ReloadCount();
                });
                return _clearEndPriceCommand;
            }
        }

        private MvxCommand _resetFiltersCommand;

        public ICommand ResetFiltersCommand
        {
            get
            {
                _resetFiltersCommand = _resetFiltersCommand ?? new MvxCommand(async () =>
                {
                    ProductFilter.Country = null;
                    ProductFilter.City = null;
                    ProductFilter.Category = null;
                    ProductFilter.Subcategory = null;
                    ProductFilter.Tags.Clear();
                    ProductFilter.StartPrice = null;
                    ProductFilter.EndPrice = null;
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
                Count = await _productManager.Count(Query, ProductFilter);
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
                await reloadSubcategories();
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
                if (ProductFilter.Country == null)
                {
                    Cities = null;
                    ProductFilter.City = null;
                }
                else if (!(ProductFilter.Country == null || Countries == null || Countries.Length == 0) &&
                         !(Cities != null && Cities.Length > 0 && Cities[0].CountryId == ProductFilter.Country.ID))
                {
                    try
                    {
                        var result = await _cityManager.Fetch(ProductFilter.Country.ID);
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
                var cityID = ProductFilter.City?.ID;
                if (string.IsNullOrEmpty(ProductFilter.City?.Name) &&
                    !string.IsNullOrEmpty(cityID) && Cities != null)
                {
                    var city = Cities.FirstOrDefault(c => c.ID == cityID);
                    if (city != null)
                        ProductFilter.City = city;
                }
            }
            StopLoading(failureMessage);
        }

        private async Task reloadSubcategories()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            using (var writerLock = await _subcategoriesLock.WriterLockAsync())
            {
                if (ProductFilter.Category == null)
                {
                    Subcategories = null;
                    ProductFilter.Subcategory = null;
                }
                else if (!(ProductFilter.Category == null || Categories == null || Categories.Length == 0) &&
                         !(Subcategories != null && Subcategories.Length > 0 &&
                           Subcategories[0].CategoryID == ProductFilter.Category.ID))
                {
                    try
                    {
                        var result = await _subcategoryManager.Fetch(ProductFilter.Category.ID);
                        Subcategories = result != null ? result.ToArray() : null;
                    }
                    catch (Exception ex)
                    {
                        failureMessage = handleException(ex);
                    }
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
            var countryID = ProductFilter.Country?.ID;
            if (string.IsNullOrEmpty(ProductFilter.Country?.Name) &&
                !string.IsNullOrEmpty(countryID) && Countries != null)
            {
                var country = Countries.FirstOrDefault(c => c.ID == countryID);
                if (country != null)
                    ProductFilter.Country = country;
            }
        }

        private async void ProductFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
            {
                RaisePropertyChanged(() => IsCitiesPickerEnabled);
                await reloadCities();
            }
            else if (e.PropertyName == "Category")
            {
                RaisePropertyChanged(() => IsSubcategoriesPickerEnabled);
                await reloadSubcategories();
            }
            await ReloadCount();
        }

        private async void tagsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
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
                Tools.Logger.Log(ex, "Error occured while loading data in product filter screen.", LoggingLevel.Error,
                    true);
                return AppResources.MessageLoadingFailed;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tojeero.Forms.Resources;
using System.Linq;
using Nito.AsyncEx;
using Tojeero.Core.Toolbox;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;
using Tojeero.Forms;

namespace Tojeero.Core.ViewModels
{
    public class FilterProductsViewModel : LoadableNetworkViewModel, IDisposable
    {
        #region Private fields

        private readonly IProductManager _productManager;
        private readonly IProductCategoryManager _categoryManager;
        private readonly IProductSubcategoryManager _subcategoryManager;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;
        private readonly ITagManager _tagManager;
        private AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();
        private AsyncReaderWriterLock _subcategoriesLock = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public FilterProductsViewModel(IProductManager productManager,
                                       IProductCategoryManager categoryManager,
                                       IProductSubcategoryManager subcategoryManager,
                                       ICountryManager countryManager,
                                       ICityManager cityManager,
                                       ITagManager tagManager)
            : base()
        {
            this._productManager = productManager;
            this._tagManager = tagManager;
            this._cityManager = cityManager;
            this._countryManager = countryManager;
            this._subcategoryManager = subcategoryManager;
            this._categoryManager = categoryManager;
            this.ProductFilter.PropertyChanged += ProductFilter_PropertyChanged;
            this.ProductFilter.Tags.CollectionChanged += tagsChanged;
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
            get
            {
                return _query;
            }
            set
            {
                _query = value;
                RaisePropertyChanged(() => Query);
            }
        }

        private int _count = -1;

        public int Count
        {
            get
            {
                return _count;
            }
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
                return string.Format(AppResources.MessageMatchingProducts, ((decimal)Count).GetShortString());
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
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCategoryFacets
        {
            get
            {
                return () => _categoryManager.GetFacets(this.Query, this.ProductFilter);
            }
        }

        private IProductSubcategory[] _subcategories;

        public IProductSubcategory[] Subcategories
        {
            get
            {
                return _subcategories;
            }
            set
            {
                _subcategories = value;
                RaisePropertyChanged(() => Subcategories);
                RaisePropertyChanged(() => IsSubcategoriesPickerEnabled);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchSubcategoryFacets
        {
            get
            {
                return () => _subcategoryManager.GetFacets(this.Query, this.ProductFilter);
            }
        }

        public bool IsSubcategoriesPickerEnabled => ProductFilter.Category != null && Subcategories != null && this.Subcategories.Length > 0;

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
                return () => _countryManager.GetProductCountryFacets(this.Query, this.ProductFilter);
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
                RaisePropertyChanged(() => IsCitiesPickerEnabled);
            }
        }

        public Func<Task<Dictionary<string, int>>> FetchCityFacets
        {
            get
            {
                return () => _cityManager.GetProductCityFacets(this.Query, this.ProductFilter);
            }
        }

        public bool IsCitiesPickerEnabled => ProductFilter.Country != null && this.Cities != null && this.Cities.Length > 0;

        private ObservableCollection<ITag> _tags;

        public ObservableCollection<ITag> Tags
        {
            get
            {
                return _tags;
            }
            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
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
                        await reload();
                    }, () => !IsLoading);
                return _reloadCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _doneCommand;

        public System.Windows.Input.ICommand DoneCommand
        {
            get
            {
                _doneCommand = _doneCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
                    {
                        RuntimeSettings.ProductFilter = this.ProductFilter;
                    });
                return _doneCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearCountryCommand;

        public System.Windows.Input.ICommand ClearCountryCommand
        {
            get
            {
                _clearCountryCommand = _clearCountryCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.Country = null;
                        await ReloadCount();
                    });
                return _clearCountryCommand;
            }
        }


        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearCityCommand;

        public System.Windows.Input.ICommand ClearCityCommand
        {
            get
            {
                _clearCityCommand = _clearCityCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.City = null;
                        await ReloadCount();
                    });
                return _clearCityCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearCategoryCommand;

        public System.Windows.Input.ICommand ClearCategoryCommand
        {
            get
            {
                _clearCategoryCommand = _clearCategoryCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.Category = null;
                        await ReloadCount();
                    });
                return _clearCategoryCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearSubcategoryCommand;

        public System.Windows.Input.ICommand ClearSubcategoryCommand
        {
            get
            {
                _clearSubcategoryCommand = _clearSubcategoryCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.Subcategory = null;
                        await ReloadCount();
                    });
                return _clearSubcategoryCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearStartPriceCommand;

        public System.Windows.Input.ICommand ClearStartPriceCommand
        {
            get
            {
                _clearStartPriceCommand = _clearStartPriceCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.StartPrice = null;
                        await ReloadCount();
                    }, () => true);
                return _clearStartPriceCommand;
            }
        }

        private Cirrious.MvvmCross.ViewModels.MvxCommand _clearEndPriceCommand;

        public System.Windows.Input.ICommand ClearEndPriceCommand
        {
            get
            {
                _clearEndPriceCommand = _clearEndPriceCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
                    {
                        this.ProductFilter.EndPrice = null;
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
                   this.ProductFilter.Country = null;
                   this.ProductFilter.City = null;
                   this.ProductFilter.Category = null;
                   this.ProductFilter.Subcategory = null;
                   this.ProductFilter.Tags.Clear();
                   this.ProductFilter.StartPrice = null;
                   this.ProductFilter.EndPrice = null;
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
                this.Count = await _productManager.Count(this.Query, this.ProductFilter);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading matching products count", LoggingLevel.Error, true);
                this.Count = -1;
            }
            _loadingCountLabel = null;
            RaisePropertyChanged(() => CountLabel);
        }

        #endregion

        #region Protected API

        protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
        {
            base.handleNetworkConnectionChanged(sender, e);
            if (e.IsConnected)
            {
                this.ReloadCommand.Execute(null);
            }
        }

        #endregion

        #region Utility methods

        private async Task reload()
        {
            this.StartLoading(AppResources.MessageGeneralLoading);
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
            this.StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            using (var writerLock = await _citiesLock.WriterLockAsync())
            {
                if (this.ProductFilter.Country == null)
                {
                    this.Cities = null;
                    this.ProductFilter.City = null;
                }
                else if (!(this.ProductFilter.Country == null || this.Countries == null || this.Countries.Length == 0) &&
                         !(this.Cities != null && this.Cities.Length > 0 && this.Cities[0].CountryId == this.ProductFilter.Country.ID))
                {
                    try
                    {
                        var result = await _cityManager.Fetch(this.ProductFilter.Country.ID);
                        this.Cities = result != null ? result.ToArray() : null;
                    }
                    catch (Exception ex)
                    {
                        failureMessage = handleException(ex);
                    }
                }
                //If the city name is empty it means it has not been loaded from backend
                //We need to update the product filter to include newly loaded data, 
                //so the user will see the city name
                var cityID = this.ProductFilter.City?.ID;
                if (string.IsNullOrEmpty(this.ProductFilter.City?.Name) &&
                    !string.IsNullOrEmpty(cityID) && this.Cities != null)
                {
                    var city = this.Cities.FirstOrDefault(c => c.ID == cityID);
                    if (city != null)
                        this.ProductFilter.City = city;
                }
            }
            StopLoading(failureMessage);
        }

        private async Task reloadSubcategories()
        {
            this.StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            using (var writerLock = await _subcategoriesLock.WriterLockAsync())
            {
                if (this.ProductFilter.Category == null)
                {
                    this.Subcategories = null;
                    this.ProductFilter.Subcategory = null;
                }
                else if (!(this.ProductFilter.Category == null || this.Categories == null || this.Categories.Length == 0) &&
                         !(this.Subcategories != null && this.Subcategories.Length > 0 && this.Subcategories[0].CategoryID == this.ProductFilter.Category.ID))
                {
                    try
                    {
                        var result = await _subcategoryManager.Fetch(this.ProductFilter.Category.ID);
                        this.Subcategories = result != null ? result.ToArray() : null;
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
            if (this.Categories != null && this.Categories.Length > 0)
                return;
            var result = await _categoryManager.Fetch();
            this.Categories = result != null ? result.ToArray() : null;
        }

        private async Task loadCountries()
        {
            if (this.Countries != null && this.Countries.Length > 0)
                return;
            var result = await _countryManager.Fetch();
            this.Countries = result != null ? result.ToArray() : null;
            //If the country name is empty it means it has not been loaded from backend
            //We need to update the product filter to include newly loaded data, 
            //so the user will see the country name
            var countryID = this.ProductFilter.Country?.ID;
            if (string.IsNullOrEmpty(this.ProductFilter.Country?.Name) &&
                !string.IsNullOrEmpty(countryID) && this.Countries != null)
            {
                var country = this.Countries.FirstOrDefault(c => c.ID == countryID);
                if (country != null)
                    this.ProductFilter.Country = country;
            }
        }

        private async void ProductFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                Tools.Logger.Log(ex, "Error occured while loading data in product filter screen.", LoggingLevel.Error, true);
                return AppResources.MessageLoadingFailed;
            }
        }

        #endregion
    }
}


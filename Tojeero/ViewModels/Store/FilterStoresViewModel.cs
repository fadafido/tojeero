using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Tojeero.Forms.Resources;
using System.Linq;
using Nito.AsyncEx;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class FilterStoresViewModel : LoadableNetworkViewModel, IDisposable
	{
		#region Private fields

		private readonly IStoreManager _storeManager;
		private readonly IStoreCategoryManager _categoryManager;
		private readonly ICountryManager _countryManager;
		private readonly ICityManager _cityManager;
		private readonly ITagManager _tagManager;
		private AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public FilterStoresViewModel(IStoreManager storeManager,
		                             IStoreCategoryManager categoryManager,
		                             ICountryManager countryManager, 
		                             ICityManager cityManager, 
		                             ITagManager tagManager)
			: base()
		{
			this._storeManager = storeManager;
			this._tagManager = tagManager;
			this._cityManager = cityManager;
			this._countryManager = countryManager;
			this._categoryManager = categoryManager;
			this.StoreFilter.PropertyChanged += StoreFilter_PropertyChanged;
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
				if (!string.IsNullOrEmpty(_loadingCountLabel))
					return _loadingCountLabel;
				if (Count < 0)
					return "";
			    if (Count == 0)
			        return AppResources.MessageNoMatchingStores;
                if (Count == 1)
                    return AppResources.MessageSingleMatchingStore;
                return string.Format(AppResources.MessageMatchingStores, ((decimal)Count).GetShortString());
            }
		}

		private IStoreCategory[] _categories;

		public IStoreCategory[] Categories
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

		public Func<Task<Dictionary<string,int>>> FetchCategoryFacets
		{
			get
			{
				return () => _categoryManager.GetFacets(this.Query, this.StoreFilter);
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

		public Func<Task<Dictionary<string,int>>> FetchCountryFacets
		{
			get
			{
				return () => _countryManager.GetStoreCountryFacets(this.Query, this.StoreFilter);
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
			}
		}

		public Func<Task<Dictionary<string,int>>> FetchCityFacets
		{
			get
			{
				return () => _cityManager.GetStoreCityFacets(this.Query, this.StoreFilter);
			}
		}

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
						RuntimeSettings.StoreFilter = this.StoreFilter;
					});
				return _doneCommand;
			}
		}

		public async Task ReloadCount()
		{
			_loadingCountLabel = "Loading matches...";
			RaisePropertyChanged(() => CountLabel);
			try
			{
				this.Count = await _storeManager.Count(this.Query, this.StoreFilter);
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
				if (!(this.StoreFilter.Country == null || this.Countries == null || this.Countries.Length == 0) &&
				    !(this.Cities != null && this.Cities.Length > 0 && this.Cities[0].CountryId == this.StoreFilter.Country.ID))
				{
					try
					{
						var result = await _cityManager.Fetch(this.StoreFilter.Country.ID);
						this.Cities = result != null ? result.ToArray() : null;
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
		}

		private async void StoreFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Country")
			{
				await reloadCities();
			}
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
				Tools.Logger.Log(ex, "Error occured while loading data in store filter screen.", LoggingLevel.Error, true);
				return AppResources.MessageLoadingFailed;
			}
		}

		#endregion
	}
}


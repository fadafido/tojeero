using System;
using Cirrious.CrossCore;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using System.Linq;
using Tojeero.Forms.Resources;
using Nito.AsyncEx;

namespace Tojeero.Core.ViewModels
{
	public class SaveStoreViewModel : BaseUserViewModel, ISaveStoreViewModel
	{
		#region Private fields and properties

		private readonly IStoreManager _storeManager;
		private readonly IStoreCategoryManager _categoryManager;
		private readonly ICountryManager _countryManager;
		private readonly ICityManager _cityManager;
		private AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public SaveStoreViewModel(IStoreManager storeManager, IStoreCategoryManager categoryManager, ICountryManager countryManager, 
			ICityManager cityManager, IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
			this._storeManager = storeManager;
			this._cityManager = cityManager;
			this._countryManager = countryManager;
			this._categoryManager = categoryManager;
			this.PropertyChanged += propertyChanged;
		}

		#endregion

		#region ISaveStoreViewModel implementation

		#region Properties

		private IStore _currentStore;

		public IStore CurrentStore
		{ 
			get
			{
				return _currentStore; 
			}
			set
			{
				_currentStore = value; 
				RaisePropertyChanged(() => CurrentStore); 
				updateViewModel();
			}
		}

		public bool HasChanged
		{
			get
			{
				return true;
			}
		}

		private string _name;

		public string Name
		{ 
			get
			{
				return _name; 
			}
			set
			{
				_name = value; 
				RaisePropertyChanged(() => Name); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		private string _description;

		public string Description
		{ 
			get
			{
				return _description; 
			}
			set
			{
				_description = value; 
				RaisePropertyChanged(() => Description); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		private string _deliveryNotes;

		public string DeliveryNotes
		{ 
			get
			{
				return _deliveryNotes; 
			}
			set
			{
				_deliveryNotes = value; 
				RaisePropertyChanged(() => DeliveryNotes); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		private IImageViewModel _mainImage;

		public IImageViewModel MainImage
		{ 
			get
			{
				return _mainImage; 
			}
			set
			{
				_mainImage = value; 
				RaisePropertyChanged(() => MainImage); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		private IStoreCategory _category;

		public IStoreCategory Category
		{ 
			get
			{
				return _category; 
			}
			set
			{
				_category = value; 
				RaisePropertyChanged(() => Category); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
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
				_country = value; 
				RaisePropertyChanged(() => Country); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
				reloadCities();
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
				_city = value; 
				RaisePropertyChanged(() => City); 
				RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		public bool IsCityEnabled
		{
			get
			{ 
				return this.Country != null && this.Cities != null && this.Cities.Length > 0;
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _saveCommand;

		public System.Windows.Input.ICommand SaveCommand
		{
			get
			{
				_saveCommand = _saveCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await save();
				}, () => CanExecuteSaveCommand);
				return _saveCommand;
			}
		}

		public bool CanExecuteSaveCommand
		{
			get
			{
				return this.IsLoggedIn && this.IsNetworkAvailable && !IsLoading && validate();
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _pickMainImage;

		public System.Windows.Input.ICommand PickMainImage
		{
			get
			{
				_pickMainImage = _pickMainImage ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					
				});
				return _pickMainImage;
			}
		}

		#endregion

		#endregion

		#region Properties

		private IStoreCategory[] _categories;

		public IStoreCategory[] Categories
		{ 
			get
			{
				return _categories; 
			}
			private set
			{
				_categories = value; 
				RaisePropertyChanged(() => Categories); 
			}
		}

		private ICountry[] _countries;

		public ICountry[] Countries
		{ 
			get
			{
				return _countries; 
			}
			private set
			{
				_countries = value; 
				RaisePropertyChanged(() => Countries); 
			}
		}

		private ICity[] _cities;

		public ICity[] Cities
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
			
		private bool _isUpdate;

		public bool IsUpdate
		{ 
			get
			{
				return _isUpdate; 
			}
			set
			{
				_isUpdate = value; 
				RaisePropertyChanged(() => IsUpdate); 
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

		#endregion

		#region Utility methods

		private void updateViewModel()
		{
			if (this.CurrentStore == null)
				nullifyViewModel();
			//TODO:Update image

			this.Name = this.CurrentStore.Name;
			this.Description = this.CurrentStore.Description;
			this.DeliveryNotes = this.CurrentStore.DeliveryNotes;
			this.Country = this.CurrentStore.Country;
			this.City = this.CurrentStore.City;
		}

		private void nullifyViewModel()
		{
			this.MainImage = null;
			this.Name = null;
			this.Description = null;
			this.DeliveryNotes = null;
			this.Country = null;
			this.City = null;
		}

		private async Task save()
		{
			StartLoading(AppResources.MessageSavingStore);
			string failureMessage = null;
			try
			{
				this.CurrentStore = await _storeManager.Save(this);
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				failureMessage = AppResources.MessageSubmissionTimeoutFailure;
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while saving store.", LoggingLevel.Error, true);
				failureMessage = AppResources.MessageSubmissionUnknownFailure;
			}
			StopLoading(failureMessage);
		}

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
				if (!(this.Country == null || this.Countries == null || this.Countries.Length == 0) &&
					!(this.Cities != null && this.Cities.Length > 0 && this.Cities[0].CountryId == this.Country.ID))
				{
					try
					{
						var result = await _cityManager.Fetch(this.Country.ID);
						this.Cities = result != null ? result.ToArray() : null;
					}
					catch (Exception ex)
					{
						failureMessage = handleException(ex);
					}
				}
			}
			StopLoading(failureMessage);
			RaisePropertyChanged(() => IsCityEnabled);
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
				Tools.Logger.Log(ex, "Error occured while loading data in save store screen.", LoggingLevel.Error, true);
				return AppResources.MessageLoadingFailed;
			}
		}

		private bool validate()
		{
			return true;
		}

		private void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty || e.PropertyName == IsLoadingProperty)
			{				
				this.RaisePropertyChanged(() => CanExecuteSaveCommand);
			}
		}

		#endregion
	}
}


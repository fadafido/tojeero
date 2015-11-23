using System;
using Cirrious.CrossCore;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using System.Linq;
using Tojeero.Forms.Resources;
using Nito.AsyncEx;
using Tojeero.Core.Toolbox;
using System.Text.RegularExpressions;

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
		private Regex _nameValidationRegex = new Regex(@"[^A-Za-z0-9\u0600-\u06FF \-_'&]");
		private Regex _whitespaceRegex = new Regex(@"\s+");

		#endregion

		#region Constructors

		public SaveStoreViewModel(IStoreManager storeManager, IStoreCategoryManager categoryManager, ICountryManager countryManager, 
		                          ICityManager cityManager, IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
			this.ShouldSubscribeToSessionChange = true;
			this._storeManager = storeManager;
			this._cityManager = cityManager;
			this._countryManager = countryManager;
			this._categoryManager = categoryManager;
			this.PropertyChanged += propertyChanged;
			this.MainImage = new ImageViewModel();
		}

		#endregion

		#region ISaveStoreViewModel implementation

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
				RaisePropertyChanged(() => Title);
				RaisePropertyChanged(() => IsNew);
				RaisePropertyChanged(() => SaveCommandTitle);
				updateViewModel();
			}
		}

		public bool IsNew
		{
			get
			{
				return this.CurrentStore == null;
			}
		}

		public bool HasChanged
		{
			get
			{
				if (this.CurrentStore == null &&
				    (Name != null || Category != null ||
				    Country != null || City != null ||
				    MainImage.NewImage != null || Description != null ||
				    DeliveryNotes != null))
				{
					return true;
				}
				else if (CurrentStore != null && 
					(Name != CurrentStore.Name || MainImage.NewImage != null ||
						Category != null && Category.ID != CurrentStore.CategoryID ||
						Country != null && Country.ID != CurrentStore.CountryId ||
						City != null && City.ID != CurrentStore.CityId ||
						DeliveryNotes != CurrentStore.DeliveryNotes))
				{
					return true;
				}
				return false;
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
				validateName();
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
				validateCategory();
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
				validateCountry();
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
				validateCity();
			}
		}

		#endregion

		#region Properties

		public string Title
		{ 
			get
			{
				string title;

				if (!this.IsNew)
				{
					title = !string.IsNullOrEmpty(this.CurrentStore.Name) ? this.CurrentStore.Name : AppResources.TitleEditStore;
				}
				else
				{
					title = AppResources.TitleCreateStore;
				}
				return title.Truncate(20);
			}
		}

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

		private bool _isPickingImage;

		public bool IsPickingImage
		{ 
			get
			{
				return _isPickingImage; 
			}
			set
			{
				_isPickingImage = value; 
				RaisePropertyChanged(() => IsPickingImage); 
			}
		}

		public bool IsCityEnabled
		{
			get
			{ 
				return this.Country != null && this.Cities != null && this.Cities.Length > 0;
			}
		}

		private string _nameInvalid;

		public string NameInvalid
		{ 
			get
			{
				return _nameInvalid; 
			}
			set
			{
				_nameInvalid = value; 
				RaisePropertyChanged(() => NameInvalid); 
			}
		}

		private string _categoryInvalid;

		public string CategoryInvalid
		{ 
			get
			{
				return _categoryInvalid; 
			}
			set
			{
				_categoryInvalid = value; 
				RaisePropertyChanged(() => CategoryInvalid); 
			}
		}

		private string _cityInvalid;

		public string CityInvalid
		{ 
			get
			{
				return _cityInvalid; 
			}
			set
			{
				_cityInvalid = value; 
				RaisePropertyChanged(() => CityInvalid); 
			}
		}

		private string _countryInvalid;

		public string CountryInvalid
		{ 
			get
			{
				return _countryInvalid; 
			}
			set
			{
				_countryInvalid = value; 
				RaisePropertyChanged(() => CountryInvalid); 
			}
		}

		public static string IsValidForSavingProperty = "IsValidForSaving";

		public bool IsValidForSaving
		{ 
			get
			{
				return NameInvalid == null && CategoryInvalid == null && CountryInvalid == null && CityInvalid == null; 
			}
		}

		public string SaveCommandTitle
		{
			get
			{
				return this.IsNew ? AppResources.ButtonCreateStore : AppResources.ButtonSaveChanges;
			}
		}

		private bool _savingInProgress;
		public static string SavingInProgressProperty = "SavingInProgress";

		public bool SavingInProgress
		{ 
			get
			{
				return _savingInProgress; 
			}
			set
			{
				_savingInProgress = value; 
				RaisePropertyChanged(() => SavingInProgress); 
			}
		}

		private string _savingFailure;

		public string SavingFailure
		{ 
			get
			{
				return _savingFailure; 
			}
			set
			{
				_savingFailure = value; 
				RaisePropertyChanged(() => SavingFailure); 
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

		private Cirrious.MvvmCross.ViewModels.MvxCommand _saveCommand;

		public System.Windows.Input.ICommand SaveCommand
		{
			get
			{
				_saveCommand = _saveCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						this.SavingFailure = null;
						if (validate() && CanExecuteSaveCommand && HasChanged)
						{
							await save();
						}	
					});
				return _saveCommand;
			}
		}

		public bool CanExecuteSaveCommand
		{
			get
			{
				return this.IsLoggedIn && this.IsNetworkAvailable && !IsLoading && !SavingInProgress && IsValidForSaving;
			}
		}

		#endregion

		#region Utility methods

		private void updateViewModel()
		{
			if (this.CurrentStore == null)
			{
				nullifyViewModel();
				return;
			}
			
			this.MainImage.ImageUrl = this.CurrentStore.ImageUrl;
			this.Name = this.CurrentStore.Name;
			this.Description = this.CurrentStore.Description;
			this.DeliveryNotes = this.CurrentStore.DeliveryNotes;
			this.Country = this.CurrentStore.Country;
			this.City = this.CurrentStore.City;
			this.Category = this.CurrentStore.Category;
		}

		private void nullifyViewModel()
		{
			this.MainImage.NewImage = null;
			this.MainImage.ImageUrl = null;
			this.Name = null;
			this.Description = null;
			this.DeliveryNotes = null;
			this.Country = null;
			this.City = null;
		}

		private async Task save()
		{
			this.SavingInProgress = true;
			string failureMessage = null;
			try
			{
				//Replace whitespaces with space
				this.Name = _whitespaceRegex.Replace(this.Name, " ").Trim();
				var nameIsReserved = await _storeManager.CheckNameIsReserved(this.Name, this.CurrentStore != null ? this.CurrentStore.ID : null);
				if (nameIsReserved)
				{
					failureMessage = AppResources.MessageValidateStoreNameReserved;
				}
				else
				{
					this.CurrentStore = await _storeManager.Save(this);
				}
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
			this.SavingFailure = failureMessage;
			this.SavingInProgress = false;
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
			validateName();
			validateCategory();
			validateCountry();
			validateCity();
			return IsValidForSaving;
		}

		private void validateName()
		{
			string invalid = null;
			if (string.IsNullOrEmpty(this.Name) ||
			    this.Name.Length < 6 || this.Name.Length > 40 ||
			    this._nameValidationRegex.IsMatch(this.Name))
			{
				invalid = AppResources.MessageValidateStoreName;
			}
			this.NameInvalid = invalid;
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void validateCategory()
		{
			this.CategoryInvalid = this.Category == null ? AppResources.MessageValidateRequiredCategory : null; 
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void validateCountry()
		{
			this.CountryInvalid = this.Country == null ? AppResources.MessageValidateRequiredCountry : null; 
			RaisePropertyChanged(() => IsValidForSaving);	
		}

		private void validateCity()
		{
			this.CityInvalid = this.City == null ? AppResources.MessageValidateRequiredCity : null; 
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
			    e.PropertyName == IsLoadingProperty || e.PropertyName == IsValidForSavingProperty ||
			    e.PropertyName == SavingInProgressProperty)
			{				
				this.RaisePropertyChanged(() => CanExecuteSaveCommand);

			}
		}

		#endregion
	}
}


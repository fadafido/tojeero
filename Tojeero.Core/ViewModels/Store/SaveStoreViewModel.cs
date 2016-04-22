using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Core.ViewModels.Image;

namespace Tojeero.Core.ViewModels.Store
{
    public class SaveStoreViewModel : BaseUserViewModel, ISaveStoreViewModel
    {
        #region Private fields and properties

        private readonly IStoreManager _storeManager;
        private readonly IStoreCategoryManager _categoryManager;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;
        private readonly AsyncReaderWriterLock _citiesLock = new AsyncReaderWriterLock();
        private readonly Regex _nameValidationRegex = new Regex(@"[^A-Za-z0-9\u0600-\u06FF \-_'&]");
        private readonly Regex _whitespaceRegex = new Regex(@"\s+");

        #endregion

        #region Constructors

        public SaveStoreViewModel(IStoreManager storeManager, IStoreCategoryManager categoryManager,
            ICountryManager countryManager,
            ICityManager cityManager, IAuthenticationService authService, IMvxMessenger messenger)
            : base(authService, messenger)
        {
            ShouldSubscribeToSessionChange = true;
            _storeManager = storeManager;
            _cityManager = cityManager;
            _countryManager = countryManager;
            _categoryManager = categoryManager;
            PropertyChanged += propertyChanged;
            MainImage = new ImageViewModel();
        }

        #endregion

        #region ISaveStoreViewModel implementation

        private IStore _currentStore;

        public IStore CurrentStore
        {
            get { return _currentStore; }
            set
            {
                _currentStore = value;
                RaisePropertyChanged(() => CurrentStore);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => IsNew);
                RaisePropertyChanged(() => IsCityEnabled);
                RaisePropertyChanged(() => SaveCommandTitle);
                updateViewModel();
            }
        }

        public bool IsNew
        {
            get { return CurrentStore == null; }
        }

        public bool HasChanged
        {
            get
            {
                if (CurrentStore == null &&
                    (Name != null || Category != null ||
                     Country != null || City != null ||
                     MainImage.NewImage != null || Description != null ||
                     DeliveryNotes != null))
                {
                    return true;
                }
                if (CurrentStore != null &&
                    (Name != CurrentStore.Name || MainImage.NewImage != null ||
                     Category != null && Category.ID != CurrentStore.CategoryID ||
                     Country != null && Country.ID != CurrentStore.CountryId ||
                     City != null && City.ID != CurrentStore.CityId ||
                     Description != CurrentStore.Description ||
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
            get { return _name; }
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
            get { return _description; }
            set
            {
                _description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        private string _deliveryNotes;

        public string DeliveryNotes
        {
            get { return _deliveryNotes; }
            set
            {
                _deliveryNotes = value;
                RaisePropertyChanged(() => DeliveryNotes);
            }
        }

        private IImageViewModel _mainImage;

        public IImageViewModel MainImage
        {
            get { return _mainImage; }
            set
            {
                _mainImage = value;
                RaisePropertyChanged(() => MainImage);
            }
        }

        private IStoreCategory _category;

        public IStoreCategory Category
        {
            get { return _category; }
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
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    _country = value;
                    RaisePropertyChanged(() => Country);
                    validateCountry();
                    Cities = null;
                    City = null;
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
                _city = value;
                RaisePropertyChanged(() => City);
                validateCity();
            }
        }

        #endregion

        #region Properties

        public Action<string, string, string> ShowAlert { get; set; }
        //Action which will called as soon as store will be saved. 
        //Bool parameter indicates wether this was new store creation or update of existing store
        public Action<IStore, bool> DidSaveStoreAction { get; set; }

        public string Title
        {
            get
            {
                string title;

                if (!IsNew)
                {
                    title = !string.IsNullOrEmpty(CurrentStore.Name) ? CurrentStore.Name : AppResources.TitleEditStore;
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
            get { return _categories; }
            private set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        private ICountry[] _countries;

        public ICountry[] Countries
        {
            get { return _countries; }
            private set
            {
                _countries = value;
                RaisePropertyChanged(() => Countries);
            }
        }

        private ICity[] _cities;

        public ICity[] Cities
        {
            get { return _cities; }
            private set
            {
                _cities = value;
                RaisePropertyChanged(() => Cities);
            }
        }

        private bool _isUpdate;

        public bool IsUpdate
        {
            get { return _isUpdate; }
            set
            {
                _isUpdate = value;
                RaisePropertyChanged(() => IsUpdate);
            }
        }

        public bool IsCityEnabled
        {
            get { return IsNew && Country != null && Cities != null && Cities.Length > 0; }
        }

        private string _nameInvalid;

        public string NameInvalid
        {
            get { return _nameInvalid; }
            set
            {
                _nameInvalid = value;
                RaisePropertyChanged(() => NameInvalid);
            }
        }

        private string _categoryInvalid;

        public string CategoryInvalid
        {
            get { return _categoryInvalid; }
            set
            {
                _categoryInvalid = value;
                RaisePropertyChanged(() => CategoryInvalid);
            }
        }

        private string _cityInvalid;

        public string CityInvalid
        {
            get { return _cityInvalid; }
            set
            {
                _cityInvalid = value;
                RaisePropertyChanged(() => CityInvalid);
            }
        }

        private string _countryInvalid;

        public string CountryInvalid
        {
            get { return _countryInvalid; }
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
            get { return IsNew ? AppResources.ButtonCreateStore : AppResources.ButtonSaveChanges; }
        }

        private bool _savingInProgress;
        public static string SavingInProgressProperty = "SavingInProgress";

        public bool SavingInProgress
        {
            get { return _savingInProgress; }
            set
            {
                _savingInProgress = value;
                RaisePropertyChanged(() => SavingInProgress);
            }
        }

        private string _savingFailure;

        public string SavingFailure
        {
            get { return _savingFailure; }
            set
            {
                _savingFailure = value;
                RaisePropertyChanged(() => SavingFailure);
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

        private MvxCommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new MvxCommand(async () =>
                {
                    SavingFailure = null;
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
            get { return IsLoggedIn && IsNetworkAvailable && !IsLoading && !SavingInProgress && IsValidForSaving; }
        }

        #endregion

        #region Utility methods

        private void updateViewModel()
        {
            if (CurrentStore == null)
            {
                nullifyViewModel();
                return;
            }

            MainImage.ImageUrl = CurrentStore.ImageUrl;
            Name = CurrentStore.Name;
            Description = CurrentStore.Description;
            DeliveryNotes = CurrentStore.DeliveryNotes;
            Country = CurrentStore.Country;
            City = CurrentStore.City;
            Category = CurrentStore.Category;
        }

        private void nullifyViewModel()
        {
            MainImage.NewImage = null;
            MainImage.ImageUrl = null;
            Name = null;
            Description = null;
            DeliveryNotes = null;
            Country = null;
            City = null;
        }

        private async Task save()
        {
            SavingInProgress = true;
            string failureMessage = null;
            try
            {
                var wasNew = IsNew;
                //Replace whitespaces with space
                Name = _whitespaceRegex.Replace(Name, " ").Trim();
                var nameIsReserved =
                    await _storeManager.CheckNameIsReserved(Name, CurrentStore != null ? CurrentStore.ID : null);
                if (nameIsReserved)
                {
                    failureMessage = AppResources.MessageValidateStoreNameReserved;
                }
                else
                {
                    CurrentStore = await _storeManager.Save(this);
                }
                if (CurrentStore != null && DidSaveStoreAction != null)
                {
                    DidSaveStoreAction(CurrentStore, wasNew);
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
            SavingFailure = failureMessage;
            if (failureMessage != null && ShowAlert != null)
            {
                ShowAlert(AppResources.MessageSavingFailure, failureMessage, AppResources.ButtonOK);
            }
            SavingInProgress = false;
        }

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
            if (Cities != null && Cities.Length > 0)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            using (var writerLock = await _citiesLock.WriterLockAsync())
            {
                if (!(Country == null || Countries == null || Countries.Length == 0) &&
                    !(Cities != null && Cities.Length > 0 && Cities[0].CountryId == Country.ID))
                {
                    try
                    {
                        var result = await _cityManager.Fetch(Country.ID);
                        Cities = result != null ? result.ToArray() : null;
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
            if (string.IsNullOrEmpty(Name) ||
                Name.Length < 6 || Name.Length > 40 ||
                _nameValidationRegex.IsMatch(Name))
            {
                invalid = AppResources.MessageValidateStoreName;
            }
            NameInvalid = invalid;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validateCategory()
        {
            CategoryInvalid = Category == null ? AppResources.MessageValidateRequiredCategory : null;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validateCountry()
        {
            CountryInvalid = Country == null ? AppResources.MessageValidateRequiredCountry : null;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validateCity()
        {
            CityInvalid = City == null ? AppResources.MessageValidateRequiredCity : null;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == IsLoadingProperty || e.PropertyName == IsValidForSavingProperty ||
                e.PropertyName == SavingInProgressProperty)
            {
                RaisePropertyChanged(() => CanExecuteSaveCommand);
            }
        }

        #endregion
    }
}
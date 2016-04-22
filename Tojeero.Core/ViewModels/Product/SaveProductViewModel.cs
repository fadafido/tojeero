using System;
using System.Collections.ObjectModel;
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

namespace Tojeero.Core.ViewModels.Product
{
    public class SaveProductViewModel : BaseUserViewModel, ISaveProductViewModel
    {
        #region Private fields and properties

        private readonly IProductManager _productManager;
        private readonly IProductCategoryManager _categoryManager;
        private readonly IProductSubcategoryManager _subcategoryManager;

        private readonly AsyncReaderWriterLock _subcategoriesLocker = new AsyncReaderWriterLock();
        private Regex _whitespaceRegex = new Regex(@"\s+");
        private readonly Regex _doubleRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

        #endregion

        #region Constructors

        public SaveProductViewModel(IProductManager productManager, IProductCategoryManager categoryManager,
            IProductSubcategoryManager subcategoryManager,
            IAuthenticationService authService, IMvxMessenger messenger)
            : base(authService, messenger)
        {
            ShouldSubscribeToSessionChange = true;
            _productManager = productManager;
            _categoryManager = categoryManager;
            _subcategoryManager = subcategoryManager;
            PropertyChanged += propertyChanged;
            MainImage = new ImageViewModel();
            validatePrice();
        }

        #endregion

        #region ISaveProductViewModel implementation

        private IProduct _currentProduct;

        public IProduct CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                _currentProduct = value;
                RaisePropertyChanged(() => CurrentProduct);
                RaisePropertyChanged(() => Title);
                RaisePropertyChanged(() => IsNew);
                RaisePropertyChanged(() => SaveCommandTitle);
                updateViewModel();
            }
        }

        private IStore _store;

        public IStore Store
        {
            get { return _store; }
            set
            {
                _store = value;
                if (_store != null)
                    _store.FetchCountry();
                RaisePropertyChanged(() => Store);
            }
        }

        public bool IsNew
        {
            get { return CurrentProduct == null; }
        }

        public bool HasChanged
        {
            get
            {
                if (CurrentProduct == null &&
                    (Name != null || Category != null || Subcategory != null ||
                     MainImage.NewImage != null || Description != null || Tags.Count > 0 ||
                     Images.Count > 0))
                {
                    return true;
                }
                if (CurrentProduct != null &&
                    (Name != CurrentProduct.Name || MainImage.NewImage != null ||
                     Category != null && Category.ID != CurrentProduct.CategoryID ||
                     Subcategory != null && Subcategory.ID != CurrentProduct.SubcategoryID ||
                     Description != CurrentProduct.Description ||
                     CurrentProduct.TagList != string.Join(", ", Tags) ||
                     !Visible != CurrentProduct.NotVisible ||
                     Price != CurrentProduct.Price ||
                     checkImagesChanged()))
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

        private double _price;

        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                RaisePropertyChanged(() => Price);
            }
        }

        private string _priceString;

        public string PriceString
        {
            get { return _priceString; }
            set
            {
                _priceString = value;
                RaisePropertyChanged(() => PriceString);
                validatePrice();
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

        private ObservableCollection<IImageViewModel> _images;

        public ObservableCollection<IImageViewModel> Images
        {
            get
            {
                if (_images == null)
                    _images = new ObservableCollection<IImageViewModel>();
                return _images;
            }
            set
            {
                _images = value;
                RaisePropertyChanged(() => Images);
            }
        }

        private IProductCategory _category;

        public IProductCategory Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    _category = value;
                    RaisePropertyChanged(() => Category);
                    RaisePropertyChanged(() => IsSubcategoryEnabled);
                    validateCategory();
                    Subcategories = null;
                    Subcategory = null;
                    reloadSubcategories();
                }
            }
        }

        private IProductSubcategory _subcategory;

        public IProductSubcategory Subcategory
        {
            get { return _subcategory; }
            set
            {
                _subcategory = value;
                RaisePropertyChanged(() => Subcategory);
                validateSubcategory();
            }
        }

        private ObservableCollection<string> _tags;

        public ObservableCollection<string> Tags
        {
            get
            {
                if (_tags == null)
                    _tags = new ObservableCollection<string>();
                return _tags;
            }
            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        private bool _isVisible;

        public bool Visible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                RaisePropertyChanged(() => Visible);
            }
        }

        #endregion

        #region Properties

        public Action<string, string, string> ShowAlert { get; set; }
        //Action which will called as soon as product will be saved. 
        //Bool parameter indicates wether this was new product creation or update of existing product
        public Action<IProduct, bool> DidSaveProductAction { get; set; }

        public string Title
        {
            get
            {
                string title;

                if (!IsNew)
                {
                    title = !string.IsNullOrEmpty(CurrentProduct.Name)
                        ? CurrentProduct.Name
                        : AppResources.TitleEditProduct;
                }
                else
                {
                    title = AppResources.TitleCreateProduct;
                }
                return title.Truncate(20);
            }
        }

        private IProductCategory[] _categories;

        public IProductCategory[] Categories
        {
            get { return _categories; }
            private set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        private IProductSubcategory[] _subcategories;

        public IProductSubcategory[] Subcategories
        {
            get { return _subcategories; }
            private set
            {
                _subcategories = value;
                RaisePropertyChanged(() => Subcategories);
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

        public bool IsSubcategoryEnabled
        {
            get { return Category != null && Subcategories != null && Subcategories.Length > 0; }
        }

        public bool IsCategoryEnabled
        {
            get { return Categories != null && Categories.Length > 0; }
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

        private string _priceInvalid;

        public string PriceInvalid
        {
            get { return _priceInvalid; }
            set
            {
                _priceInvalid = value;
                RaisePropertyChanged(() => PriceInvalid);
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

        private string _subcategoryInvalid;

        public string SubcategoryInvalid
        {
            get { return _subcategoryInvalid; }
            set
            {
                _subcategoryInvalid = value;
                RaisePropertyChanged(() => SubcategoryInvalid);
            }
        }

        public static string IsValidForSavingProperty = "IsValidForSaving";

        public bool IsValidForSaving
        {
            get
            {
                return NameInvalid == null && PriceInvalid == null && CategoryInvalid == null &&
                       SubcategoryInvalid == null;
            }
        }

        public string SaveCommandTitle
        {
            get { return IsNew ? AppResources.ButtonCreateProduct : AppResources.ButtonSaveChanges; }
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
                    var wasNew = IsNew;
                    if (validate() && CanExecuteSaveCommand && HasChanged)
                    {
                        await save();
                    }
                    if (CurrentProduct != null && DidSaveProductAction != null)
                    {
                        DidSaveProductAction(CurrentProduct, wasNew);
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

        #region Methods

        public IImageViewModel ImageViewModelFactory()
        {
            var image = new ImageViewModel();
            image.RemoveImageAction = () => removeImage(image);
            //Can only pick image if it wasn't picked previously
            image.CanPickImage = () => image.ImageID == null;
            return image;
        }

        #endregion

        #region Utility methods

        private void updateViewModel()
        {
            if (CurrentProduct == null)
            {
                nullifyViewModel();
                return;
            }

            MainImage.ImageUrl = CurrentProduct.ImageUrl;
            Name = CurrentProduct.Name;
            PriceString = CurrentProduct.Price.ToString();
            Category = CurrentProduct.Category;
            Subcategory = CurrentProduct.Subcategory;
            Description = CurrentProduct.Description;
            Tags = null;
            Tags.AddRange(CurrentProduct.Tags);
            Visible = !CurrentProduct.NotVisible;
        }

        private void nullifyViewModel()
        {
            MainImage.NewImage = null;
            MainImage.ImageUrl = null;
            Name = null;
            PriceString = null;
            Description = null;
            Category = null;
            Subcategory = null;
            Visible = true;
        }

        private async Task save()
        {
            SavingInProgress = true;
            string failureMessage = null;
            try
            {
                CurrentProduct = await _productManager.Save(this);
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                failureMessage = AppResources.MessageSubmissionTimeoutFailure;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occured while saving product.", LoggingLevel.Error, true);
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
                await loadCategories();
                await reloadSubcategories();
                await loadImages();
            }
            catch (Exception ex)
            {
                failureMessage = handleException(ex);
            }
            StopLoading(failureMessage);
        }

        private async Task loadCategories()
        {
            if (Categories != null && Categories.Length > 0)
                return;
            var result = await _categoryManager.Fetch();
            Categories = result != null ? result.ToArray() : null;
            RaisePropertyChanged(() => IsCategoryEnabled);
        }

        private async Task reloadSubcategories()
        {
            if (Subcategories != null && Subcategories.Length > 0)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            RaisePropertyChanged(() => IsSubcategoryEnabled);

            string failureMessage = null;
            using (var writerLock = await _subcategoriesLocker.WriterLockAsync())
            {
                if (!(Category == null || Categories == null || Categories.Length == 0) &&
                    !(Subcategories != null && Subcategories.Length > 0 && Subcategories[0].ID == Category.ID))
                {
                    try
                    {
                        var result = await _subcategoryManager.Fetch(Category.ID);
                        Subcategories = result != null ? result.ToArray() : null;
                    }
                    catch (Exception ex)
                    {
                        failureMessage = handleException(ex);
                    }
                }
            }
            StopLoading(failureMessage);
            RaisePropertyChanged(() => IsSubcategoryEnabled);
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
                Tools.Logger.Log(ex, "Error occured while loading data in save product screen.", LoggingLevel.Error,
                    true);
                return AppResources.MessageLoadingFailed;
            }
        }

        private bool validate()
        {
            validateName();
            validateCategory();
            validateSubcategory();
            return IsValidForSaving;
        }

        private void validateName()
        {
            string invalid = null;
            if (string.IsNullOrEmpty(Name) ||
                Name.Length < 5 || Name.Length > 256)
            {
                invalid = AppResources.MessageValidateProductName;
            }
            NameInvalid = invalid;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validatePrice()
        {
            if (PriceString != null && !_doubleRegex.IsMatch(PriceString))
                PriceInvalid = AppResources.MessageValidationPriceInvalid;
            else
                PriceInvalid = null;
            double price = 0;
            if (PriceInvalid == null && PriceString != null)
            {
                double.TryParse(PriceString, out price);
            }
            Price = price;
            PriceInvalid = Price <= 0 ? AppResources.MessageValidationPriceInvalid : null;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validateCategory()
        {
            CategoryInvalid = Category == null ? AppResources.MessageValidateRequiredCategory : null;
            RaisePropertyChanged(() => IsValidForSaving);
        }

        private void validateSubcategory()
        {
            SubcategoryInvalid = Subcategory == null ? AppResources.MessageValidateRequiredSubcategory : null;
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

        private async Task loadImages()
        {
            if (CurrentProduct == null || Images != null && Images.Count > 0)
                return;
            var images = await CurrentProduct.GetImages();
            if (images != null)
            {
                Images = new ObservableCollection<IImageViewModel>();
                foreach (var image in images)
                {
                    var imageViewModel = ImageViewModelFactory();
                    imageViewModel.ImageID = image.ID;
                    imageViewModel.ImageUrl = image.Url;
                    Images.Add(imageViewModel);
                }
            }
        }

        private async Task<bool> removeImage(IImageViewModel image)
        {
            if (image == null)
                return false;

            string failureMessage = null;
            try
            {
                //If there is current set product and this image is coming from already
                //existing file, i.e. it has set ID, then we should remove it from product relation
                //and then only from Images collection
                if (CurrentProduct != null && !string.IsNullOrEmpty(image.ImageID))
                {
                    await CurrentProduct.RemoveImage(image.ImageID);
                }
                Images.Remove(image);
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occured while removing image.", LoggingLevel.Error, true);
                failureMessage = AppResources.MessageRemoveImageFailure;
            }

            if (failureMessage != null && ShowAlert != null)
            {
                ShowAlert(AppResources.TitleFailure, failureMessage, AppResources.ButtonOK);
            }
            return failureMessage == null;
        }

        private bool checkImagesChanged()
        {
            foreach (var image in Images)
            {
                if (image.NewImage != null)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
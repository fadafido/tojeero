using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
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

		private AsyncReaderWriterLock _subcategoriesLocker = new AsyncReaderWriterLock();
		private Regex _whitespaceRegex = new Regex(@"\s+");
		private Regex _doubleRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

		#endregion

		#region Constructors

		public SaveProductViewModel(IProductManager productManager, IProductCategoryManager categoryManager, IProductSubcategoryManager subcategoryManager,
			IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
			this.ShouldSubscribeToSessionChange = true;
			this._productManager = productManager;
			this._categoryManager = categoryManager;
			this._subcategoryManager = subcategoryManager;
			this.PropertyChanged += propertyChanged;
			this.MainImage = new ImageViewModel();
			validatePrice();
		}

		#endregion

		#region ISaveProductViewModel implementation

		private IProduct _currentProduct;

		public IProduct CurrentProduct
		{ 
			get
			{
				return _currentProduct; 
			}
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
			get
			{
				return _store; 
			}
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
			get
			{
				return this.CurrentProduct == null;
			}
		}

		public bool HasChanged
		{
			get
			{
				if (this.CurrentProduct == null &&
					(Name != null || Category != null || Subcategory != null ||
						MainImage.NewImage != null || Description != null || this.Tags.Count > 0 ||
						this.Images.Count > 0))
				{
					return true;
				}
				else if (CurrentProduct != null && 
					(Name != CurrentProduct.Name || MainImage.NewImage != null ||
						Category != null && Category.ID != CurrentProduct.CategoryID ||
						Subcategory != null && Subcategory.ID != CurrentProduct.SubcategoryID ||
						Description != CurrentProduct.Description ||
						this.CurrentProduct.TagList != string.Join(", ", Tags) ||
						!this.Visible != this.CurrentProduct.NotVisible ||
						this.Price != this.CurrentProduct.Price ||
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

		private double _price;

		public double Price
		{ 
			get
			{
				return _price; 
			}
			set
			{
				_price = value; 
				RaisePropertyChanged(() => Price); 
			}
		}

		private string _priceString;

		public string PriceString
		{ 
			get
			{
				return _priceString; 
			}
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

		private ObservableCollection<IImageViewModel> _images;

		public ObservableCollection<IImageViewModel> Images
		{ 
			get
			{
				if(_images == null)
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
			get
			{
				return _category; 
			}
			set
			{
			    if (_category != value)
			    {
			        _category = value;
			        RaisePropertyChanged(() => Category);
			        RaisePropertyChanged(() => IsSubcategoryEnabled);
			        validateCategory();
			        this.Subcategories = null;
			        this.Subcategory = null;
			        reloadSubcategories();
			    }
			}
		}

		private IProductSubcategory _subcategory;

		public IProductSubcategory Subcategory
		{ 
			get
			{
				return _subcategory; 
			}
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
			get
			{
				return _isVisible; 
			}
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

				if (!this.IsNew)
				{
					title = !string.IsNullOrEmpty(this.CurrentProduct.Name) ? this.CurrentProduct.Name : AppResources.TitleEditProduct;
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

		private IProductSubcategory[] _subcategories;

		public IProductSubcategory[] Subcategories
		{ 
			get
			{
				return _subcategories; 
			}
			private set
			{
				_subcategories = value; 
				RaisePropertyChanged(() => Subcategories); 
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

		public bool IsSubcategoryEnabled
		{
			get
			{ 
				return this.Category != null && this.Subcategories != null && this.Subcategories.Length > 0;
			}
		}

		public bool IsCategoryEnabled
		{
			get
			{ 
				return this.Categories != null && this.Categories.Length > 0;
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

		private string _priceInvalid;

		public string PriceInvalid
		{ 
			get
			{
				return _priceInvalid; 
			}
			set
			{
				_priceInvalid = value; 
				RaisePropertyChanged(() => PriceInvalid); 
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

		private string _subcategoryInvalid;

		public string SubcategoryInvalid
		{ 
			get
			{
				return _subcategoryInvalid; 
			}
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
				return NameInvalid == null && PriceInvalid == null && CategoryInvalid == null && SubcategoryInvalid == null; 
			}
		}

		public string SaveCommandTitle
		{
			get
			{
				return this.IsNew ? AppResources.ButtonCreateProduct : AppResources.ButtonSaveChanges;
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
						bool wasNew = this.IsNew;
						if (validate() && CanExecuteSaveCommand && HasChanged)
						{
							await save();
						}	
						if(this.CurrentProduct != null && DidSaveProductAction != null)
						{
							DidSaveProductAction(this.CurrentProduct, wasNew);
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
			if (this.CurrentProduct == null)
			{
				nullifyViewModel();
				return;
			}

			this.MainImage.ImageUrl = this.CurrentProduct.ImageUrl;
			this.Name = this.CurrentProduct.Name;
			this.PriceString = this.CurrentProduct.Price.ToString();
			this.Category = this.CurrentProduct.Category;
			this.Subcategory = this.CurrentProduct.Subcategory;
			this.Description = this.CurrentProduct.Description;
			this.Tags = null;
			this.Tags.AddRange(this.CurrentProduct.Tags);
			this.Visible = !this.CurrentProduct.NotVisible;
		}

		private void nullifyViewModel()
		{
			this.MainImage.NewImage = null;
			this.MainImage.ImageUrl = null;
			this.Name = null;
			this.PriceString = null;
			this.Description = null;
			this.Category = null;
			this.Subcategory = null;
			this.Visible = true;
		}

		private async Task save()
		{
			this.SavingInProgress = true;
			string failureMessage = null;
			try
			{
				
				this.CurrentProduct = await _productManager.Save(this);
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
			this.SavingFailure = failureMessage;
			if (failureMessage != null && this.ShowAlert != null)
			{
				this.ShowAlert(AppResources.MessageSavingFailure, failureMessage, AppResources.ButtonOK);
			}
			this.SavingInProgress = false;
		}

		private async Task reload()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
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
			if (this.Categories != null && this.Categories.Length > 0)
				return;
			var result = await _categoryManager.Fetch();
			this.Categories = result != null ? result.ToArray() : null;
			RaisePropertyChanged(() => IsCategoryEnabled);
		}

		private async Task reloadSubcategories()
		{
            if (this.Subcategories != null && this.Subcategories.Length > 0)
                return;
            this.StartLoading(AppResources.MessageGeneralLoading);
			RaisePropertyChanged(() => IsSubcategoryEnabled);

			string failureMessage = null;
			using (var writerLock = await _subcategoriesLocker.WriterLockAsync())
			{
				if (!(this.Category == null || this.Categories == null || this.Categories.Length == 0) &&
					!(this.Subcategories != null && this.Subcategories.Length > 0 && this.Subcategories[0].ID == this.Category.ID))
				{
					try
					{
						var result = await _subcategoryManager.Fetch(this.Category.ID);
						this.Subcategories = result != null ? result.ToArray() : null;
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
				Tools.Logger.Log(ex, "Error occured while loading data in save product screen.", LoggingLevel.Error, true);
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
			if (string.IsNullOrEmpty(this.Name) ||
				this.Name.Length < 5 || this.Name.Length > 256)
			{
				invalid = AppResources.MessageValidateProductName;
			}
			this.NameInvalid = invalid;
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void validatePrice()
		{
			if (this.PriceString != null && !_doubleRegex.IsMatch(this.PriceString))
				this.PriceInvalid = AppResources.MessageValidationPriceInvalid;
			else
				this.PriceInvalid = null;
			double price = 0;
			if (this.PriceInvalid == null && this.PriceString != null)
			{
				double.TryParse(this.PriceString as string, out price);
			}
			this.Price = price;
			this.PriceInvalid = this.Price <= 0 ? AppResources.MessageValidationPriceInvalid : null;
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void validateCategory()
		{
			this.CategoryInvalid = this.Category == null ? AppResources.MessageValidateRequiredCategory : null; 
			RaisePropertyChanged(() => IsValidForSaving);
		}

		private void validateSubcategory()
		{
			this.SubcategoryInvalid = this.Subcategory == null ? AppResources.MessageValidateRequiredSubcategory : null; 
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

		private async Task loadImages()
		{
			if (this.CurrentProduct == null || this.Images != null && this.Images.Count > 0)
				return;
			var images = await this.CurrentProduct.GetImages();
			if (images != null)
			{
				this.Images = new ObservableCollection<IImageViewModel>();
				foreach (var image in images)
				{
					var imageViewModel = ImageViewModelFactory();
					imageViewModel.ImageID = image.ID;
					imageViewModel.ImageUrl = image.Url;
					this.Images.Add(imageViewModel);
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
				if(this.CurrentProduct != null && !string.IsNullOrEmpty(image.ImageID))
				{
					await this.CurrentProduct.RemoveImage(image.ImageID);
				}
				this.Images.Remove(image);
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while removing image.", LoggingLevel.Error, true);
				failureMessage = AppResources.MessageRemoveImageFailure;
			}

			if (failureMessage != null && this.ShowAlert != null)
			{
				this.ShowAlert(AppResources.TitleFailure, failureMessage, AppResources.ButtonOK);
			}
			return failureMessage == null;
		}
			
		private bool checkImagesChanged()
		{
			foreach (var image in this.Images)
			{
				if (image.NewImage != null)
					return true;
			}
			return false;
		}

		#endregion
	}
}


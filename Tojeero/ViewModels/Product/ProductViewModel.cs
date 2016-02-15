using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class ProductViewModel : BaseUserViewModel, ISocialViewModel
	{
		#region Private fields and properties

		AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public ProductViewModel(IProduct product = null)
			: base(Mvx.Resolve<IAuthenticationService>(), Mvx.Resolve<IMvxMessenger>())
		{
			this.Product = product;
			this.PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public static string ProductProperty = "Product";
		private IProduct _product;

		public IProduct Product
		{ 
			get
			{
				return _product; 
			}
			set
			{
				if (_product != null)
					_product.PropertyChanged -= propertyChanged;
				_product = value; 
				if (_product != null)
					_product.PropertyChanged += propertyChanged;
				RaisePropertyChanged(() => Product); 
				this.LoadFavoriteCommand.Execute(null);
			}
		}

		public virtual string StatusWarning
		{
			get
			{
				string warning = null;
				if (this.Product != null)
				{
					if (Product.IsBlocked)
					{
						warning = AppResources.LabelBlocked;
					}
					else
					{
						switch (this.Product.Status)
						{
							case ProductStatus.Pending:
								warning = AppResources.LabelPending;
								break;
							case ProductStatus.Declined:
								warning = AppResources.LabelDeclined;
								break;
						}
					}
				}
				return warning;
			}
		}

		public Xamarin.Forms.Color WarningColor
		{
			get
			{
				var color = Xamarin.Forms.Color.Transparent;
				if (this.Product != null)
				{
					if (this.Product.IsBlocked)
						color = Tojeero.Forms.Colors.Invalid;
					if (this.Product.Status == ProductStatus.Pending)
						color = Tojeero.Forms.Colors.Warning;
					else if (this.Product.Status == ProductStatus.Declined)
						color = Tojeero.Forms.Colors.Invalid;
				}
				return color;
			}
		}

		private IFavorite _favorite;

		public IFavorite Favorite
		{ 
			get
			{
				return _favorite; 
			}
			set
			{
				_favorite = value; 
				RaisePropertyChanged(() => Favorite); 
				RaisePropertyChanged(() => IsFavoriteToggleVisible); 
			}
		}

		public bool IsFavoriteToggleVisible
		{
			get
			{
				return this.Favorite != null && FavoriteToggleEnabled;
			}
		}

	    private bool _favoriteToggleEnabled = true;
	    public bool FavoriteToggleEnabled
        { 
	        get  
	        {
	            return _favoriteToggleEnabled; 
	        }
	        set 
	        {
	            _favoriteToggleEnabled = value; 
	            RaisePropertyChanged(() => FavoriteToggleEnabled);
                RaisePropertyChanged(() => IsFavoriteToggleVisible);
                RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
            }
	    }  

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFavoriteCommand;

		public System.Windows.Input.ICommand LoadFavoriteCommand
		{
			get
			{
				_loadFavoriteCommand = _loadFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await loadFavorite();
					}, () => CanExecuteLoadFavoriteCommand);
				return _loadFavoriteCommand;
			}
		}

		public static string CanExecuteLoadFavoriteCommandProperty = "CanExecuteLoadFavoriteCommand";

		public bool CanExecuteLoadFavoriteCommand
		{
			get
			{
				return this.FavoriteToggleEnabled && 
                    this.Product != null && 
                    this.Product.ID != null && 
                    this.Favorite == null && 
                    this.IsNetworkAvailable && 
                    this.IsLoggedIn;
			}
		}


		private Cirrious.MvvmCross.ViewModels.MvxCommand _toggleFavoriteCommand;

		public System.Windows.Input.ICommand ToggleFavoriteCommand
		{
			get
			{
				_toggleFavoriteCommand = _toggleFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await toggleFavorite();
					}, () => CanExecuteToggleFavoriteCommand);
				return _toggleFavoriteCommand;
			}
		}

		public static string CanExecuteToggleFavoriteCommandProperty = "CanExecuteToggleFavoriteCommand";

		public bool CanExecuteToggleFavoriteCommand
		{
			get
			{
				return this.Product != null && this.Product.ID != null && this.Favorite != null && this.IsNetworkAvailable && !this.IsLoading && this.IsLoggedIn;
			}
		}

		#endregion

		#region Utility methods

		protected async Task loadFavorite()
		{
			if (!CanExecuteLoadFavoriteCommand)
				return;
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					this.Favorite = await _authService.CurrentUser.GetProductFavorite(this.Product.ID);
				}
				catch (Exception ex)
				{
					failureMessage = "Failed to load favorite.";
					Tools.Logger.Log(ex, "Failed to load favorite for product with ID '{0}'", LoggingLevel.Error, true, this.Product.ID);
				}
			}
			StopLoading(failureMessage);
		}

		protected async Task toggleFavorite()
		{
			if (!CanExecuteToggleFavoriteCommand)
				return;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					if (this.Favorite.IsFavorite)
					{
						await _authService.CurrentUser.RemoveProductFromFavorites(this.Product.ID);
					}
					else
					{
						await _authService.CurrentUser.AddProductToFavorites(this.Product.ID);
					}
				}
				catch (Exception ex)
				{
					Tools.Logger.Log(ex, "Failed to load favorite for product with ID '{0}'", LoggingLevel.Error, true, this.Product.ID);
				}
			}
		}

		protected virtual void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
			    e.PropertyName == "Favorite" || e.PropertyName == IsLoadingProperty)
			{				
				this.RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
			}

			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
			    e.PropertyName == ProductProperty)
			{
				this.RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
			}		

			if (e.PropertyName == CanExecuteLoadFavoriteCommandProperty)
			{
				this.LoadFavoriteCommand.Execute(null);
			}

			//If the user state has changed to logged off then we need to clean the favorite state
			if (e.PropertyName == IsLoggedInProperty && !this.IsLoggedIn && this.Product != null)
			{
				this.Favorite = null;
			}
		}

		#endregion
	}
}


using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tojeero.Core.ViewModels
{
	public class ProductViewModel : BaseUserViewModel
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
				return this.Product != null && this.Product.ID != null && this.Product.IsFavorite == null && this.IsNetworkAvailable && this.IsLoggedIn;
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
				return this.Product != null && this.Product.IsFavorite != null && this.IsNetworkAvailable && !this.IsLoading && this.IsLoggedIn;
			}
		}

		#endregion

		#region Utility methods

		protected async Task loadFavorite()
		{
			if (!CanExecuteLoadFavoriteCommand)
				return;
			StartLoading();
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					this.Product.IsFavorite = await _authService.CurrentUser.IsProductFavorite(this.Product.ID);
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
			StartLoading();
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					var isFav = this.Product.IsFavorite.Value;
					if(isFav)
					{
						await _authService.CurrentUser.RemoveProductFromFavorites(this.Product.ID);
					}
					else
					{
						await _authService.CurrentUser.AddProductToFavorites(this.Product.ID);
					}
					this.Product.IsFavorite = !isFav;
				}
				catch (Exception ex)
				{
					failureMessage = "Failed to toggle favorite.";
					Tools.Logger.Log(ex, "Failed to load favorite for product with ID '{0}'", LoggingLevel.Error, true, this.Product.ID);
				}
			}
			StopLoading(failureMessage);
		}

		private void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
				e.PropertyName == "IsFavorite" || e.PropertyName == IsLoadingProperty)
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
				this.Product.IsFavorite = null;
			}
		}

		#endregion
	}
}


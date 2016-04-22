using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Logging;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Contracts;
using Xamarin.Forms;

namespace Tojeero.Core.ViewModels.Product
{
    public class ProductViewModel : BaseUserViewModel, ISocialViewModel
    {
        #region Private fields and properties

        readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public ProductViewModel(IProduct product = null)
            : base(Mvx.Resolve<IAuthenticationService>(), Mvx.Resolve<IMvxMessenger>())
        {
            Product = product;
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public static string ProductProperty = "Product";
        private IProduct _product;

        public IProduct Product
        {
            get { return _product; }
            set
            {
                if (_product != null)
                    _product.PropertyChanged -= propertyChanged;
                _product = value;
                if (_product != null)
                    _product.PropertyChanged += propertyChanged;
                RaisePropertyChanged(() => Product);
                LoadFavoriteCommand.Execute(null);
            }
        }

        public virtual string StatusWarning
        {
            get
            {
                string warning = null;
                if (Product != null)
                {
                    if (Product.IsBlocked)
                    {
                        warning = AppResources.LabelBlocked;
                    }
                    else
                    {
                        switch (Product.Status)
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

        public Color WarningColor
        {
            get
            {
                var color = Color.Transparent;
                if (Product != null)
                {
                    if (Product.IsBlocked)
                        color = Colors.Invalid;
                    if (Product.Status == ProductStatus.Pending)
                        color = Colors.Warning;
                    else if (Product.Status == ProductStatus.Declined)
                        color = Colors.Invalid;
                }
                return color;
            }
        }

        private IFavorite _favorite;

        public IFavorite Favorite
        {
            get { return _favorite; }
            set
            {
                _favorite = value;
                RaisePropertyChanged(() => Favorite);
                RaisePropertyChanged(() => IsFavoriteToggleVisible);
            }
        }

        public bool IsFavoriteToggleVisible
        {
            get { return Favorite != null && FavoriteToggleEnabled; }
        }

        private bool _favoriteToggleEnabled = true;

        public bool FavoriteToggleEnabled
        {
            get { return _favoriteToggleEnabled; }
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

        private MvxCommand _loadFavoriteCommand;

        public ICommand LoadFavoriteCommand
        {
            get
            {
                _loadFavoriteCommand = _loadFavoriteCommand ??
                                       new MvxCommand(async () => { await loadFavorite(); },
                                           () => CanExecuteLoadFavoriteCommand);
                return _loadFavoriteCommand;
            }
        }

        public static string CanExecuteLoadFavoriteCommandProperty = "CanExecuteLoadFavoriteCommand";

        public bool CanExecuteLoadFavoriteCommand
        {
            get
            {
                return FavoriteToggleEnabled &&
                       Product != null &&
                       Product.ID != null &&
                       Favorite == null &&
                       IsNetworkAvailable &&
                       IsLoggedIn;
            }
        }


        private MvxCommand _toggleFavoriteCommand;

        public ICommand ToggleFavoriteCommand
        {
            get
            {
                _toggleFavoriteCommand = _toggleFavoriteCommand ??
                                         new MvxCommand(async () => { await toggleFavorite(); },
                                             () => CanExecuteToggleFavoriteCommand);
                return _toggleFavoriteCommand;
            }
        }

        public static string CanExecuteToggleFavoriteCommandProperty = "CanExecuteToggleFavoriteCommand";

        public bool CanExecuteToggleFavoriteCommand
        {
            get
            {
                return Product != null && Product.ID != null && Favorite != null && IsNetworkAvailable && !IsLoading &&
                       IsLoggedIn;
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
                    Favorite = await _authService.CurrentUser.GetProductFavorite(Product.ID);
                }
                catch (Exception ex)
                {
                    failureMessage = "Failed to load favorite.";
                    Tools.Logger.Log(ex, "Failed to load favorite for product with ID '{0}'", LoggingLevel.Error, true,
                        Product.ID);
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
                    if (Favorite.IsFavorite)
                    {
                        await _authService.CurrentUser.RemoveProductFromFavorites(Product.ID);
                    }
                    else
                    {
                        await _authService.CurrentUser.AddProductToFavorites(Product.ID);
                    }
                }
                catch (Exception ex)
                {
                    Tools.Logger.Log(ex, "Failed to load favorite for product with ID '{0}'", LoggingLevel.Error, true,
                        Product.ID);
                }
            }
        }

        protected virtual void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == "Favorite" || e.PropertyName == IsLoadingProperty)
            {
                RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
            }

            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == ProductProperty)
            {
                RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
            }

            if (e.PropertyName == CanExecuteLoadFavoriteCommandProperty)
            {
                LoadFavoriteCommand.Execute(null);
            }

            //If the user state has changed to logged off then we need to clean the favorite state
            if (e.PropertyName == IsLoggedInProperty && !IsLoggedIn && Product != null)
            {
                Favorite = null;
            }
        }

        #endregion
    }
}
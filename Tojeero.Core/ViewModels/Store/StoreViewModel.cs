using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Store
{
    public class StoreViewModel : BaseUserViewModel, ISocialViewModel
    {
        #region Private fields and properties

        readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public StoreViewModel(IStore store = null)
            : base(Mvx.Resolve<IAuthenticationService>(), Mvx.Resolve<IMvxMessenger>())
        {
            Store = store;
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public static string StoreProperty = "Store";
        private IStore _store;

        public virtual IStore Store
        {
            get { return _store; }
            set
            {
                if (_store != null)
                    _store.PropertyChanged -= propertyChanged;
                _store = value;
                if (_store != null)
                    _store.PropertyChanged += propertyChanged;
                RaisePropertyChanged(() => Store);
                LoadFavoriteCommand.Execute(null);
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
            get { return Favorite != null; }
        }

        public virtual string StatusWarning
        {
            get
            {
                string warning = null;
                if (Store != null)
                {
                    if (Store.IsBlocked)
                        warning = AppResources.MessageStoreBlocked;
                }
                return warning;
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
            get { return Store != null && Store.ID != null && Favorite == null && IsNetworkAvailable && IsLoggedIn; }
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
                return Store != null && Store.ID != null && Favorite != null && IsNetworkAvailable && !IsLoading &&
                       IsLoggedIn;
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
                    Favorite = await _authService.CurrentUser.GetStoreFavorite(Store.ID);
                }
                catch (Exception ex)
                {
                    failureMessage = "Failed to load favorite.";
                    Tools.Logger.Log(ex, "Failed to load favorite for store with ID '{0}'", LoggingLevel.Error, true,
                        Store.ID);
                }
            }
            StopLoading(failureMessage);
        }

        private async Task toggleFavorite()
        {
            if (!CanExecuteToggleFavoriteCommand)
                return;
            using (var writerLock = await _locker.WriterLockAsync())
            {
                try
                {
                    if (Favorite.IsFavorite)
                    {
                        await _authService.CurrentUser.RemoveStoreFromFavorites(Store.ID);
                    }
                    else
                    {
                        await _authService.CurrentUser.AddStoreToFavorites(Store.ID);
                    }
                }
                catch (Exception ex)
                {
                    Tools.Logger.Log(ex, "Failed to load favorite for store with ID '{0}'", LoggingLevel.Error, true,
                        Store.ID);
                }
            }
        }

        private void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == "Favorite" || e.PropertyName == IsLoadingProperty || e.PropertyName == "")
            {
                RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
            }

            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == StoreProperty || e.PropertyName == "")
            {
                RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
            }

            if (e.PropertyName == CanExecuteLoadFavoriteCommandProperty || e.PropertyName == "")
            {
                LoadFavoriteCommand.Execute(null);
            }

            if (e.PropertyName == "Status" || e.PropertyName == "Mode" || e.PropertyName == "")
            {
                RaisePropertyChanged(() => StatusWarning);
            }

            //If the user state has changed to logged off then we need to clean the favorite state
            if ((e.PropertyName == IsLoggedInProperty || e.PropertyName == "") && !IsLoggedIn && Store != null)
            {
                Favorite = null;
            }
        }

        #endregion
    }
}
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Logging;
using Tojeero.Core.Messages;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseUserStoreViewModel : BaseUserViewModel
    {
        #region Private fields and properties

        private readonly MvxSubscriptionToken _languageChangeToken;
        private readonly MvxSubscriptionToken _storeChangeToken;
        private bool _isLoggedIn;

        #endregion

        #region Constructors

        public BaseUserStoreViewModel(IAuthenticationService authService, IMvxMessenger messenger)
            : base(authService, messenger)
        {
            ShouldSubscribeToSessionChange = true;

            _storeChangeToken = messenger.SubscribeOnMainThread<StoreChangedMessage>(message =>
            {
                //If the changed store is related to current user then refetch the user store to reflect the change
                if (message.Store != null && CurrentUser != null &&
                    message.Store.OwnerID == CurrentUser.ID)
                {
                    reloadUserStore();
                }
            });
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public Action<IStore> ShowSaveStoreAction { get; set; }
        public Action<IStore> DidLoadUserStoreAction { get; set; }
        public Action IsLoadingStoreAction { get; set; }


        private bool _isLoadingUserStore;
        public static string IsLoadingUserStoreProperty = "IsLoadingUserStore";

        public bool IsLoadingUserStore
        {
            get { return _isLoadingUserStore; }
            set
            {
                _isLoadingUserStore = value;
                RaisePropertyChanged(() => IsLoadingUserStore);
            }
        }

        private bool _isLoadingUserStoreFailed;
        public static string IsLoadingUserStoreFailedProperty = "IsLoadingUserStoreFailed";

        public bool IsLoadingUserStoreFailed
        {
            get { return _isLoadingUserStoreFailed; }
            set
            {
                _isLoadingUserStoreFailed = value;
                RaisePropertyChanged(() => IsLoadingUserStoreFailed);
            }
        }

        private IStore _userStore;
        public static string UserStoreProperty = "UserStore";

        public IStore UserStore
        {
            get { return _userStore; }
            set
            {
                _userStore = value;
                RaisePropertyChanged(() => UserStore);
                RaisePropertyChanged(() => ShowSaveStoreTitle);
            }
        }

        public string ShowSaveStoreTitle
        {
            get
            {
                string title;

                if (UserStore != null)
                {
                    title = AppResources.ButtonMyStore;
                }
                else
                {
                    title = AppResources.ButtonCreateStore;
                }
                return title;
            }
        }

        public bool IsShowSaveStoreVisible
        {
            get { return !IsLoadingUserStore && !IsLoadingUserStoreFailed; }
        }

        #endregion

        #region Commands

        private MvxCommand _showSaveStoreCommand;

        public ICommand ShowSaveStoreCommand
        {
            get
            {
                _showSaveStoreCommand = _showSaveStoreCommand ??
                                        new MvxCommand(() => { ShowSaveStoreAction.Fire(UserStore); });
                return _showSaveStoreCommand;
            }
        }

        private MvxCommand _loadUserStoreCommand;

        public ICommand LoadUserStoreCommand
        {
            get
            {
                _loadUserStoreCommand = _loadUserStoreCommand ??
                                        new MvxCommand(async () => { await loadUserStore(); },
                                            () => CanExecuteLoadUserStoreCommand);
                return _loadUserStoreCommand;
            }
        }

        public bool CanExecuteLoadUserStoreCommand
        {
            get { return IsLoggedIn && IsNetworkAvailable && !IsLoadingUserStore && UserStore == null; }
        }

        #endregion

        #region Utility Methods

        private async Task loadUserStore()
        {
            IsLoadingUserStore = true;
            IsLoadingUserStoreFailed = false;
            try
            {
                if (CurrentUser.DefaultStore == null)
                    await CurrentUser.LoadDefaultStore();
                UserStore = CurrentUser.DefaultStore;
                DidLoadUserStoreAction.Fire(UserStore);
            }
            catch (OperationCanceledException ex)
            {
                IsLoadingUserStoreFailed = true;
                Tools.Logger.Log(ex, LoggingLevel.Debug);
            }
            catch (Exception ex)
            {
                IsLoadingUserStoreFailed = true;
                Tools.Logger.Log(ex, LoggingLevel.Error, true);
            }
            IsLoadingUserStore = false;
        }

        void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
                e.PropertyName == IsLoadingUserStoreProperty || e.PropertyName == UserStoreProperty)
            {
                RaisePropertyChanged(() => CanExecuteLoadUserStoreCommand);
            }

            if (e.PropertyName == IsLoadingUserStoreProperty || e.PropertyName == IsLoadingUserStoreFailedProperty)
            {
                RaisePropertyChanged(() => IsShowSaveStoreVisible);
            }

            //If the logged in property has changed we need to reload user store.
            if (e.PropertyName == IsLoggedInProperty && _isLoggedIn != IsLoggedIn)
            {
                _isLoggedIn = IsLoggedIn;
                reloadUserStore();
            }
        }


        void reloadUserStore()
        {
            UserStore = null;
            IsLoadingStoreAction.Fire();
            LoadUserStoreCommand.Execute(null);
        }

        #endregion
    }
}
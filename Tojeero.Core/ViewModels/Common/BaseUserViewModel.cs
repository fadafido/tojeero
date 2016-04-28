using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseUserViewModel : BaseLoadableNetworkViewModel, IUserViewModel
    {
        #region Private Fields and Properties

        protected readonly IAuthenticationService _authService;
        protected readonly IMvxMessenger _messenger;
        MvxSubscriptionToken _userChangeToken;

        #endregion

        #region Constructors

        public BaseUserViewModel(IAuthenticationService authService, IMvxMessenger messenger)
        {
            _messenger = messenger;
            _authService = authService;
            CurrentUser = _authService.CurrentUser;
            reloadUserChangeSubscribtion();
        }

        #endregion

        #region Properties

        public static string CurrentUserProperty = "CurrentUser";
        private IUser _currentUser;

        public IUser CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                RaisePropertyChanged(() => CurrentUser);
                RaisePropertyChanged(() => IsLoggedIn);
            }
        }

        public static string IsLoggedInProperty = "IsLoggedIn";

        public bool IsLoggedIn
        {
            get { return CurrentUser != null; }
        }

        private bool _shouldSubscribeToSessionChange;

        public virtual bool ShouldSubscribeToSessionChange
        {
            get { return _shouldSubscribeToSessionChange; }
            set
            {
                if (_shouldSubscribeToSessionChange != value)
                {
                    _shouldSubscribeToSessionChange = value;
                    reloadUserChangeSubscribtion();
                }
            }
        }

        #endregion

        #region Utility methods

        void reloadUserChangeSubscribtion()
        {
            if (ShouldSubscribeToSessionChange == false && _userChangeToken != null)
            {
                _messenger.Unsubscribe<CurrentUserChangedMessage>(_userChangeToken);
                _userChangeToken = null;
            }
            else if (ShouldSubscribeToSessionChange && _userChangeToken == null)
            {
                _userChangeToken =
                    _messenger.SubscribeOnMainThread<CurrentUserChangedMessage>(
                        message => { CurrentUser = message.CurrentUser; });
            }
        }

        #endregion
    }
}
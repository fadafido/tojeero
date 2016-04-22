using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Main
{
    public class RootViewModel : MvxViewModel
    {
        #region Private fields and properties

        private readonly IAuthenticationService _authService;
        private readonly MvxSubscriptionToken _sessionChangedToken;

        #endregion

        #region Constructors

        public RootViewModel(IAuthenticationService authService, IMvxMessenger messenger)
        {
            _authService = authService;

            _sessionChangedToken =
                messenger.SubscribeOnMainThread<SessionStateChangedMessage>(m => { reloadFavorites(); });
            UserStoreViewModel = MvxToolbox.LoadViewModel<BaseUserStoreViewModel>();
        }

        #endregion

        #region Public API

        public Action ShowFavorites { get; set; }

        public Action HideFavorites { get; set; }

        private BaseUserStoreViewModel _userStoreViewModel;

        public BaseUserStoreViewModel UserStoreViewModel
        {
            get { return _userStoreViewModel; }
            set
            {
                _userStoreViewModel = value;
                RaisePropertyChanged(() => UserStoreViewModel);
            }
        }

        public void Initialize()
        {
            reloadFavorites();
            UserStoreViewModel.LoadUserStoreCommand.Execute(null);
        }

        #endregion

        #region Utility methods

        private void reloadFavorites()
        {
            if (_authService.State == SessionState.LoggedIn)
            {
                ShowFavorites.Fire();
            }
            else
            {
                HideFavorites.Fire();
            }
        }

        #endregion
    }
}
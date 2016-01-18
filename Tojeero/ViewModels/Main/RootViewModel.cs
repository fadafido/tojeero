using System;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Services;
using Tojeero.Core.Messages;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Core.ViewModels
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

			_sessionChangedToken = messenger.SubscribeOnMainThread<SessionStateChangedMessage>((m) =>
				{
					reloadFavorites();
				});
			this.UserStoreViewModel = MvxToolbox.LoadViewModel<BaseUserStoreViewModel>();
		}

		#endregion

		#region Public API

		public Action ShowFavorites { get; set; }

		public Action HideFavorites { get; set; }

		private BaseUserStoreViewModel _userStoreViewModel;

		public BaseUserStoreViewModel UserStoreViewModel
		{ 
			get
			{
				return _userStoreViewModel; 
			}
			set
			{
				_userStoreViewModel = value; 
				RaisePropertyChanged(() => UserStoreViewModel); 
			}
		}

		public void Initialize()
		{
			reloadFavorites();
			this.UserStoreViewModel.LoadUserStoreCommand.Execute(null);
		}

		#endregion

		#region Utility methods

		private void reloadFavorites()
		{
			if (_authService.State == SessionState.LoggedIn)
			{
				this.ShowFavorites.Fire();	
			}
			else
			{
				this.HideFavorites.Fire();
			}
		}


		#endregion
	}
}


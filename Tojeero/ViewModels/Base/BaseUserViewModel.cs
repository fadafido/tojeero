using System;
using System.Threading.Tasks;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using System.Collections.Generic;

namespace Tojeero.Core.ViewModels
{
	public class BaseUserViewModel : LoadableNetworkViewModel, IUserViewModel
	{
		#region Private Fields and Properties

		protected readonly IAuthenticationService _authService;
		protected readonly IMvxMessenger _messenger;
		MvxSubscriptionToken _userChangeToken;

		#endregion

		#region Constructors

		public BaseUserViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base()
		{
			this._messenger = messenger;
			this._authService = authService;
			CurrentUser = _authService.CurrentUser;
			reloadUserChangeSubscribtion();
		}

		#endregion

		#region Properties

		public static string CurrentUserProperty = "CurrentUser";
		private IUser _currentUser;
		public IUser CurrentUser
		{ 
			get
			{
				return _currentUser; 
			}
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
			get
			{
				return CurrentUser != null;
			}
		}

		private bool _shouldSubscribeToSessionChange = false;

		public virtual bool ShouldSubscribeToSessionChange
		{
			get
			{
				return _shouldSubscribeToSessionChange;
			}
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
			if (this.ShouldSubscribeToSessionChange == false && _userChangeToken != null)
			{
				_messenger.Unsubscribe<CurrentUserChangedMessage>(_userChangeToken);
				_userChangeToken = null;
			}
			else if (ShouldSubscribeToSessionChange == true && _userChangeToken == null)
			{
				_userChangeToken = _messenger.SubscribeOnMainThread<CurrentUserChangedMessage>((message) =>
					{
						CurrentUser = message.CurrentUser;
					});
			}
		}

		#endregion

	}
}


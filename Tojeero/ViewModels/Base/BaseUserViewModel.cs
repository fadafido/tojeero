using System;
using System.Threading.Tasks;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using System.Collections.Generic;

namespace Tojeero.Core.ViewModels
{
	public class BaseUserViewModel : LoadableNetworkViewModel
	{
		#region Private Fields and Properties

		protected readonly IAuthenticationService _authService;
		protected readonly IMvxMessenger _messenger;
		protected readonly List<MvxSubscriptionToken> _messengerTokens = new List<MvxSubscriptionToken>();

		#endregion

		#region Constructors

		public BaseUserViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base()
		{
			this._messenger = messenger;
			this._authService = authService;
			CurrentUser = _authService.CurrentUser;
			_messengerTokens.Add(messenger.SubscribeOnMainThread<CurrentUserChangedMessage>((message) =>
				{
					CurrentUser = message.CurrentUser;
				}));
		}

		#endregion

		#region Properties

		private User _currentUser;
		public User CurrentUser
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

		public bool IsLoggedIn
		{
			get
			{
				return CurrentUser != null;
			}
		}
		#endregion

	}
}


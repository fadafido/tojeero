using System;
using System.Threading.Tasks;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core.ViewModels
{
	public class BaseUserViewModel : LoadableViewModel
	{
		#region Private Fields and Properties

		private readonly IAuthenticationService _authService;
		private readonly IMvxMessenger _messenger;
		private readonly MvxSubscriptionToken _messengerToken;

		#endregion

		#region Constructors

		public BaseUserViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base()
		{
			this._messenger = messenger;
			this._authService = authService;
			CurrentUser = _authService.CurrentUser;
			_messengerToken = messenger.Subscribe<SessionStateChangedMessage>((message) =>
				{
					CurrentUser = _authService.CurrentUser;
				});
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


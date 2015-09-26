using System;
using Tojeero.Core;
using System.Threading.Tasks;
using Parse;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using Newtonsoft.Json;


namespace Tojeero.Core.Services
{
	public class ParseAuthenticationService : IAuthenticationService
	{
		private readonly IFacebookService _facebookService;
		private readonly IMvxMessenger _messenger;

		public ParseAuthenticationService(IFacebookService facebookService, IMvxMessenger messenger)
		{
			_messenger = messenger;
			_facebookService = facebookService;
		}


		#region IAuthenticationService implementation

		private User _currentUser;
		public User CurrentUser
		{
			get
			{
				return _currentUser;
			}
			private set
			{
				_currentUser = value;
				updateCurrentUser();
			}
		}

		private SessionState _state;
		public SessionState State
		{
			get
			{
				return _state;
			}
			private set
			{
				if (_state != value)
				{
					_state = value;
					_messenger.Publish(new SessionStateChangedMessage(this, _state));
				}
			}
		}


		public void LogOut()
		{
			_facebookService.LogOut();
			ParseUser.LogOut();
			CurrentUser = null;
			State = SessionState.LoggedOut;
		}

		public async Task<User> LogInWithFacebook()
		{
			try
			{
				var fbUser = await _facebookService.GetFacebookToken();
				if(fbUser == null)
					return null;
				
				var parseUser = await ParseFacebookUtils.LogInAsync(fbUser.User.ID, fbUser.Token, fbUser.ExpiryDate);
				parseUser.Email = fbUser.User.Email;
				parseUser["firstName"] = fbUser.User.FirstName;
				parseUser["lastName"] = fbUser.User.LastName;
				parseUser["profilePictureUri"] = fbUser.User.ProfilePictureUrl;
				await parseUser.SaveAsync();
				fbUser.User.ID = parseUser.ObjectId;
				fbUser.User.UserName = parseUser.Username;

				CurrentUser = fbUser.User;
				State = SessionState.LoggedIn;

				return fbUser.User;
			}
			catch(Exception ex)
			{
				State = SessionState.LoggedOut;
				CurrentUser = null;
				Mvx.Trace(MvxTraceLevel.Error, ex.ToString());
				return null;
			}

		}
			
		public async Task RestoreSavedSession()
		{
			await Task.Factory.StartNew(() =>
				{
					var jsonUser = Settings.CurrentUser;
					if(!string.IsNullOrEmpty(jsonUser))
					{
						CurrentUser = JsonConvert.DeserializeObject<User>(jsonUser);
					}
					this.State = CurrentUser == null ? SessionState.LoggedOut : SessionState.LoggedIn;
				});
		}

		#endregion

		#region Utility Methods

		private void updateSessionState()
		{
			
		}

		private async void updateCurrentUser()
		{
			await Task.Factory.StartNew(() =>
				{
					if(CurrentUser == null)
						Settings.CurrentUser = "";
					else
					{
						Settings.CurrentUser = JsonConvert.SerializeObject(CurrentUser);
					}
				});
		}

		#endregion
	}
}


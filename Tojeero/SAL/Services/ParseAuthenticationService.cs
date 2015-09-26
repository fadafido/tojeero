using System;
using Tojeero.Core;
using System.Threading.Tasks;
using Parse;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using Newtonsoft.Json;
using System.Threading;


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


		public async Task LogOut()
		{
			await Task.Factory.StartNew(() =>
				{
					_facebookService.LogOut();
					CurrentUser = null;
					State = SessionState.LoggedOut;
					//Do the parse logout at the last step because seems it takes a lot of time to log out when no network connection is available
					ParseUser.LogOut();
				});			
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

				CurrentUser = getCurrentUser();
				State = SessionState.LoggedIn;

				return this.CurrentUser;
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
						_currentUser = JsonConvert.DeserializeObject<User>(jsonUser);
						fireCurrentUserChanged();
					}
					this.State = CurrentUser == null ? SessionState.LoggedOut : SessionState.LoggedIn;
				});
		}

		public async Task UpdateUserDetails(User user, CancellationToken token)
		{
			var currentUser = ParseUser.CurrentUser;
			if (currentUser == null)
				return;
			currentUser["firstName"] = user.FirstName;
			currentUser["lastName"] = user.LastName;
			currentUser["country"] = user.Country;
			currentUser["city"] = user.City;
			currentUser["mobile"] = user.Mobile;
			currentUser["isProfileSubmitted"] = true;
			await currentUser.SaveAsync(token);

			this.CurrentUser = getCurrentUser();
		}

		#endregion

		#region Utility Methods

		private void updateSessionState()
		{
			
		}

		private async void updateCurrentUser()
		{
			fireCurrentUserChanged();
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

		private void fireCurrentUserChanged()
		{
			_messenger.Publish<CurrentUserChangedMessage>(new CurrentUserChangedMessage(this, this.CurrentUser));
		}

		private User getCurrentUser()
		{
			var currentUser = ParseUser.CurrentUser;
			if (currentUser == null)
				return null;	
			string first, last, pic, country, city, mobile;
			bool submitted;

			currentUser.TryGetValue<String>("firstName", out first);
			currentUser.TryGetValue<String>("lastName", out last);
			currentUser.TryGetValue<String>("profilePictureUri", out pic);
			currentUser.TryGetValue<String>("country", out country);
			currentUser.TryGetValue<String>("city", out city);
			currentUser.TryGetValue<String>("mobile", out mobile);
			currentUser.TryGetValue<bool>("isProfileSubmitted", out submitted);

			var user = new User()
				{
					ID = currentUser.ObjectId,
					UserName = currentUser.Username,
					Email = currentUser.Email,
					FirstName = first,
					LastName = last,
					ProfilePictureUrl = pic,
					Country = country,
					City = city,
					Mobile = mobile,
					IsProfileSubmitted = submitted
				};
			return user;
		}
		#endregion
	}
}


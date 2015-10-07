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
using Connectivity.Plugin;
using Xamarin;


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
			await Task.Factory.StartNew(async () =>
				{
					try
					{
						_facebookService.LogOut();
						CurrentUser = null;
						State = SessionState.LoggedOut;
						var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
						await ParseUser.LogOutAsync(tokenSource.Token);
					}
					catch(Exception ex)
					{
						handleException(ex);	
					}
				}).ConfigureAwait(false);			
		}

		public async Task<User> LogInWithFacebook()
		{
			try
			{
				var fbUser = await _facebookService.GetFacebookToken().ConfigureAwait(false);
				if(fbUser == null)
					return null;
				
				var parseUser = await ParseFacebookUtils.LogInAsync(fbUser.User.ID, fbUser.Token, fbUser.ExpiryDate);
				parseUser.Email = fbUser.User.Email;
				parseUser["firstName"] = fbUser.User.FirstName;
				parseUser["lastName"] = fbUser.User.LastName;
				parseUser["profilePictureUri"] = fbUser.User.ProfilePictureUrl;
				populateUserPreferances(parseUser);
				await parseUser.SaveAsync().ConfigureAwait(false);

				CurrentUser = getCurrentUser();
				State = SessionState.LoggedIn;

				return this.CurrentUser;
			}
			catch(Exception ex)
			{
				State = SessionState.LoggedOut;
				CurrentUser = null;
				Tools.Logger.Log(ex, LoggingLevel.Error);
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
				}).ConfigureAwait(false);
		}

		public async Task UpdateUserDetails(User user, CancellationToken token)
		{
			var currentUser = ParseUser.CurrentUser;
			if (currentUser == null)
				return;
			currentUser["firstName"] = user.FirstName;
			currentUser["lastName"] = user.LastName;
			currentUser["countryId"] = user.CountryId;
			currentUser["cityId"] = user.CityId;
			currentUser["mobile"] = user.Mobile;
			currentUser["isProfileSubmitted"] = true;
			await currentUser.SaveAsync(token).ConfigureAwait(false);

			this.CurrentUser = getCurrentUser();
		}

		//Update settings bases on user profile
		private void populateUserPreferances(ParseObject user)
		{
			//If the user has already selected country then we need to override Settings with values from user entity
			if (user.ContainsKey("countryId"))
			{
				var countryId = user.Get<int?>("countryId");
				if(countryId != null)
					Settings.CountryId = countryId;
			}
			else
			{
				user["countryId"] = Settings.CountryId;
			}

			if (user.ContainsKey("cityId"))
			{
				var cityId = user.Get<int?>("cityId");
				if(cityId != null)
					Settings.CityId = cityId;
			}
			else
			{
				user["cityId"] = Settings.CityId;
			}
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
				}).ConfigureAwait(false);
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
			string first, last, pic, mobile;
			int country, city;
			bool submitted;

			currentUser.TryGetValue<String>("firstName", out first);
			currentUser.TryGetValue<String>("lastName", out last);
			currentUser.TryGetValue<String>("profilePictureUri", out pic);
			currentUser.TryGetValue<int>("countryId", out country);
			currentUser.TryGetValue<int>("cityId", out city);
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
					CountryId = country,
					CityId = city,
					Mobile = mobile,
					IsProfileSubmitted = submitted
				};
			return user;
		}

		private void handleException(Exception exception)
		{
			try
			{
				throw exception;
			}
			catch(OperationCanceledException ex)
			{
				if (!CrossConnectivity.Current.IsConnected)
					Tools.Logger.Log(ex, TraceMessages.OperationTimeOutNoNetwork, LoggingLevel.Warning, true);
				else
				{
					string message = string.Format(TraceMessages.OperationTimeOut, ex.ToString());
					Tools.Logger.Log(ex, TraceMessages.OperationTimeOut, LoggingLevel.Error, true);
				}
			}
			catch(Exception ex)
			{

			}
		}
		#endregion
	}
}


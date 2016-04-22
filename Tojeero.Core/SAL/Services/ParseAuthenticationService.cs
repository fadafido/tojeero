using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Connectivity.Plugin;
using Newtonsoft.Json;
using Parse;
using Parse.Core.Internal;
using Tojeero.Core.Logging;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Core.Services
{
    public class ParseAuthenticationService : IAuthenticationService
    {
        #region Private fields

        private readonly IFacebookService _facebookService;
        private readonly IChatService _chatService;
        private readonly IMvxMessenger _messenger;

        #endregion

        #region Constructors

        public ParseAuthenticationService(IFacebookService facebookService, IChatService chatService,
            IMvxMessenger messenger)
        {
            _chatService = chatService;
            _messenger = messenger;
            _facebookService = facebookService;
        }

        #endregion

        #region IAuthenticationService implementation

        private User _currentUser;

        public IUser CurrentUser
        {
            get { return _currentUser; }
            private set
            {
                _currentUser = value as User;
                updateCurrentUser();
            }
        }

        private SessionState _state;

        public SessionState State
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    updateChatSession();
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
                catch (Exception ex)
                {
                    handleException(ex);
                }
            }).ConfigureAwait(false);
        }

        public async Task<IUser> LogInWithFacebook()
        {
            try
            {
                var fbUser = await _facebookService.GetFacebookToken().ConfigureAwait(false);
                if (fbUser == null)
                    return null;

                var authData = GetFacebookAuthData(fbUser.User.ID, fbUser.Token, fbUser.ExpiryDate);
                var user = await ParseUserExtensions.LogInWithAsync("facebook", authData, CancellationToken.None);
                var parseUser = (TojeeroUser) user;
                parseUser.Email = fbUser.User.Email;
                parseUser.FirstName = fbUser.User.FirstName;
                parseUser.LastName = fbUser.User.LastName;
                parseUser.ProfilePictureUrl = fbUser.User.ProfilePictureUrl;
                populateUserPreferances(parseUser);
                await parseUser.SaveAsync().ConfigureAwait(false);

                CurrentUser = new User(parseUser);
                State = SessionState.LoggedIn;

                return CurrentUser;
            }
            catch (Exception ex)
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
                if (!string.IsNullOrEmpty(jsonUser))
                {
                    _currentUser = JsonConvert.DeserializeObject<User>(jsonUser);
                    fireCurrentUserChanged();
                }
                State = CurrentUser == null ? SessionState.LoggedOut : SessionState.LoggedIn;
            }).ConfigureAwait(false);
        }

        public async Task UpdateUserDetails(IUser user, CancellationToken token)
        {
            if (_currentUser == null)
                return;
            _currentUser.FirstName = user.FirstName;
            _currentUser.LastName = user.LastName;
            _currentUser.CountryId = user.CountryId;
            _currentUser.CityId = user.CityId;
            _currentUser.Mobile = user.Mobile;
            _currentUser.IsProfileSubmitted = true;
            await _currentUser.ParseObject.SaveAsync(token).ConfigureAwait(false);
            //Update the local settings
            Settings.CityId = user.CityId;
            Settings.CountryId = user.CountryId;
            updateCurrentUser();
        }

        //Update settings bases on user profile
        private void populateUserPreferances(TojeeroUser user)
        {
            //If the user has already selected country then we need to override Settings with values from user entity
            if (user.Country != null)
            {
                Settings.CountryId = user.Country.ObjectId;
            }
            else if (Settings.CountryId != null)
            {
                user.Country = ParseObject.CreateWithoutData<ParseCountry>(Settings.CountryId);
            }

            if (user.City != null)
            {
                Settings.CityId = user.City.ObjectId;
            }
            else if (Settings.CityId != null)
            {
                user.City = ParseObject.CreateWithoutData<ParseCity>(Settings.CityId);
            }
        }

        #endregion

        #region Utility Methods

        private async void updateCurrentUser()
        {
            fireCurrentUserChanged();
            await Task.Factory.StartNew(() =>
            {
                if (CurrentUser == null)
                    Settings.CurrentUser = "";
                else
                {
                    Settings.CurrentUser = JsonConvert.SerializeObject(CurrentUser);
                }
            }).ConfigureAwait(false);
        }

        private void fireCurrentUserChanged()
        {
            _messenger.Publish(new CurrentUserChangedMessage(this, CurrentUser));
        }

        private void handleException(Exception exception)
        {
            try
            {
                throw exception;
            }
            catch (OperationCanceledException ex)
            {
                if (!CrossConnectivity.Current.IsConnected)
                    Tools.Logger.Log(ex, TraceMessages.OperationTimeOutNoNetwork, LoggingLevel.Warning, true);
                else
                {
                    var message = string.Format(TraceMessages.OperationTimeOut, ex);
                    Tools.Logger.Log(ex, TraceMessages.OperationTimeOut, LoggingLevel.Error, true);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private async Task updateChatSession()
        {
            if (State == SessionState.LoggedIn)
            {
                await _chatService.LogInAsync(CurrentUser);
            }
            else
            {
                await _chatService.LogOutAsync();
            }
        }

        private IDictionary<string, object> GetFacebookAuthData(string facebookId, string accessToken,
            DateTime expiration)
        {
            return new Dictionary<string, object>
            {
                {
                    "id",
                    facebookId
                },
                {
                    "access_token",
                    accessToken
                },
                {
                    "expiration_date",
                    expiration.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")
                }
            };
        }

        #endregion
    }
}
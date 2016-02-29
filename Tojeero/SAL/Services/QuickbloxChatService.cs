using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Nito.AsyncEx;
using Quickblox.Sdk;
using Tojeero.Core.Toolbox;
using Quickblox.Sdk.Modules.UsersModule.Requests;
using Tojeero.Core.Messages;

namespace Tojeero.Core.Services
{
    public class QuickbloxChatService : IChatService
    {
        #region Private fields and properties

        private readonly QuickbloxClient _quickblox;
        private readonly IAuthenticationService _authService;
        private AsyncReaderWriterLock _authLocker = new AsyncReaderWriterLock();
        private HMACSHA1 _hash;
        private object _sessionChangedToken;

        private HMACSHA1 Hash  
        {
            get
            {
                if(_hash == null)
                {
                    _hash = new HMACSHA1(Constants.QuickbloxAuthSecret.GetBytes());
                }
                return _hash;
            }
        }
        #endregion

        #region Constructors

        public QuickbloxChatService(IAuthenticationService authService, IMvxMessenger messenger)
        {
            _authService = authService;
            _quickblox = new QuickbloxClient(Constants.QuickbloxAppId, Constants.QuickbloxAuthKey, Constants.QuickbloxAuthSecret);
            _sessionChangedToken = messenger.Subscribe<SessionStateChangedMessage>(handleSessionStateChanged);
        }

        #endregion

        #region IChatService implementation

        public Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize)
        {
            return GetMessagesAsync(channelID, startDate, pageSize, CancellationToken.None);
        }

        public async Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize, CancellationToken token)
        {
            await loginIfRequired(token);
            throw new NotImplementedException();
        }

        public async Task SendMessageAsync(IChatMessage message, string channelID)
        {
            await SendMessageAsync(message, channelID, CancellationToken.None);
        }

        public async Task SendMessageAsync(IChatMessage message, string channelID, CancellationToken token)
        {
            await loginIfRequired(token);
            throw new NotImplementedException();
        }

        public async Task SubscribeToChannelAsync(string channelID)
        {
            await SubscribeToChannelAsync(channelID, CancellationToken.None);
        }

        public async Task SubscribeToChannelAsync(string channelID, CancellationToken token)
        {
            await loginIfRequired(token);
        }

        public async Task UnsubscribeFromChannelAsync(string channelID)
        {
            await UnsubscribeFromChannelAsync(channelID, CancellationToken.None);
            throw new NotImplementedException();
        }

        public Task UnsubscribeFromChannelAsync(string channelID, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utility methods

        private async Task loginIfRequired(CancellationToken token)
        {
            using (var writerLock = await _authLocker.WriterLockAsync())
            {
                if (_quickblox.Token != null)
                    return;
                if (_authService.CurrentUser?.ID == null)
                    throw new InvalidOperationException("You need to sign in before using chat SDK.");

                //Get password by computing secret hash of the user id
                var password = Hash.ComputeHash(_authService.CurrentUser.ID.GetBytes()).HashEncode().Substring(0, 40);
                var userID = _authService.CurrentUser.ID;

                //Try to authenticate
                var response = await _quickblox.AuthenticationClient.CreateSessionWithLoginAsync(userID, password);
                token.ThrowIfCancellationRequested();

                //If the authentication failed it means the user hasn't been created in Quickblox yet
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await signupUser(userID, password, token);
                    //If the signup succeeded (otherwise exception would be thrown) 
                    //we need to create a session with already existing user
                    response = await _quickblox.AuthenticationClient.CreateSessionWithLoginAsync(userID, password);
                    token.ThrowIfCancellationRequested();
                }

                //If the authentication failed for some reason, aka status code is not Created throw exception
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                    throw new Exception(
                        $"Unable to create chat session because Quickblox user login failed due to unknown error. " +
                        $"\n HTTP Status code: {response.StatusCode}." +
                        $"Errors: \n {getErrorDesc(response.Errors)}");
            }
        }

        private async Task signupUser(string userID, string password, CancellationToken token)
        {
            //Create base session, this is required by quickblox API before making any other request
            var sessionResponse = await _quickblox.AuthenticationClient.CreateSessionBaseAsync();
            token.ThrowIfCancellationRequested();

            //Throw exception if session hasn't been created for some reason
            if (sessionResponse.StatusCode != System.Net.HttpStatusCode.Created)
                throw new Exception($"Unable to create chat session because Quickblox session creation failed due to unknown error. " +
                                    $"\n HTTP Status code: {sessionResponse.StatusCode}." +
                                    $"Errors: \n {getErrorDesc(sessionResponse.Errors)}");

            //Create user signup request by the current user data
            var userSignUpRequest = new UserSignUpRequest
            {
                User = new UserRequest()
                {
                    Login = userID,
                    Email = _authService.CurrentUser.Email,
                    FullName = _authService.CurrentUser.FullName,
                    Password = password
                }
            };

            //Try to sign the user up
            var userResponse = await _quickblox.UsersClient.SignUpUserAsync(userSignUpRequest);
            token.ThrowIfCancellationRequested();

            //If the signup failed throw exception
            if (userResponse.StatusCode != System.Net.HttpStatusCode.Created)
                throw new Exception($"Unable to create chat session because Quickblox user signup failed due to unknown error. " +
                                    $"\n HTTP Status code: {userResponse.StatusCode}." +
                                    $"Errors: \n {getErrorDesc(userResponse.Errors)}");
        }

        private async Task logout()
        {
            using (var writerLock = await _authLocker.WriterLockAsync())
            {
                await _quickblox.AuthenticationClient.DeleteSessionAsync(_quickblox.Token);
            }
        }

        private string getErrorDesc(Dictionary<string, string[]> errors)
        {
            var errorDesc = string.Join("\n", errors.Select(e => $"{e.Key}:  {string.Join(", ", e.Value)}"));
            return errorDesc;
        }

        private async void handleSessionStateChanged(SessionStateChangedMessage m)
        {
            if (m.NewState == SessionState.LoggedOut && _quickblox.Token != null)
            {
                await logout();
            }
            else if(m.NewState == SessionState.LoggedIn && _quickblox.Token == null)
            {
                await loginIfRequired(CancellationToken.None);
            }
        }
        #endregion


    }
}

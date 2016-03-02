using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Cirrious.MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Quickblox.Sdk;
using Quickblox.Sdk.GeneralDataModel.Filter;
using Quickblox.Sdk.GeneralDataModel.Filters;
using Quickblox.Sdk.GeneralDataModel.Response;
using Quickblox.Sdk.Modules.ChatXmppModule;
using Quickblox.Sdk.Modules.UsersModule.Models;
using Tojeero.Core.Toolbox;
using Quickblox.Sdk.Modules.UsersModule.Requests;
using Quickblox.Sdk.Modules.UsersModule.Responses;
using Sharp.Xmpp;
using Tojeero.Core.Messages;

namespace Tojeero.Core.Services
{
    public class QuickbloxChatService : IChatService
    {
        #region Private fields and properties

        private readonly QuickbloxClient _quickblox;
        private AsyncReaderWriterLock _authLocker = new AsyncReaderWriterLock();
        private Dictionary<string, int> _userIDs = new Dictionary<string, int>();

        private HMACSHA1 _hash;

        private HMACSHA1 Hash
        {
            get
            {
                if (_hash == null)
                {
                    _hash = new HMACSHA1(Constants.QuickbloxAuthSecret.GetBytes());
                }
                return _hash;
            }
        }

        #endregion

        #region Constructors

        public QuickbloxChatService(IMvxMessenger messenger)
        {
            _quickblox = new QuickbloxClient(Constants.QuickbloxAppId, Constants.QuickbloxAuthKey,
                Constants.QuickbloxAuthSecret);
            _quickblox.ChatXmppClient.MessageReceived += chatMessageReceived;
            _quickblox.ChatXmppClient.ErrorReceived += chatOnErrorReceived;
        }

        #endregion

        #region IChatService implementation

        public Task SignUpAsync(IUser user)
        {
            return SignUpAsync(user, CancellationToken.None);
        }

        public async Task SignUpAsync(IUser user, CancellationToken token)
        {
            using (var writerLock = await _authLocker.WriterLockAsync(token))
            {
                await signupUser(user, token);
            }
        }

        public Task LogInAsync(IUser user)
        {
            return LogInAsync(user, CancellationToken.None);
        }

        public Task LogInAsync(IUser user, CancellationToken token)
        {
            return loginIfRequired(user, token);
        }

        public Task LogOutAsync()
        {
            return LogOutAsync(CancellationToken.None);
        }

        public async Task LogOutAsync(CancellationToken token)
        {
            using (var writerLock = await _authLocker.WriterLockAsync())
            {
                await _quickblox.AuthenticationClient.DeleteSessionAsync(_quickblox.Token);
            }
        }

        public Task<IEnumerable<IChatMessage>> GetMessagesAsync(IUser user, string channelID, DateTimeOffset? startDate, int pageSize)
        {
            return GetMessagesAsync(user, channelID, startDate, pageSize, CancellationToken.None);
        }

        public async Task<IEnumerable<IChatMessage>> GetMessagesAsync(IUser user, string channelID, DateTimeOffset? startDate, int pageSize, CancellationToken token)
        {
            await loginIfRequired(user, token);
            return new List<IChatMessage>();
        }

        public async Task SendMessageAsync(IUser sender, IChatMessage message, string channelID)
        {
            await SendMessageAsync(sender, message, channelID, CancellationToken.None);
        }

        public async Task SendMessageAsync(IUser sender, IChatMessage message, string channelID, CancellationToken token)
        {
            await loginIfRequired(sender, token);
            var jsonMessage = JsonConvert.SerializeObject(message);
            var recipientID = await getQuickbloxUserId(message.RecipientID, token);
            var extraParams = "<extraParams> " +
                                "<save_to_history>1</save_to_history> " +
                              "</extraParams>";
            _quickblox.ChatXmppClient.SendMessage(recipientID, jsonMessage, extraParams, channelID);
        }

        public async Task SubscribeToChannelAsync(IUser user, string channelID)
        {
            await SubscribeToChannelAsync(user, channelID, CancellationToken.None);
        }

        public async Task SubscribeToChannelAsync(IUser user, string channelID, CancellationToken token)
        {
            await loginIfRequired(user, token);
        }

        public async Task UnsubscribeFromChannelAsync(IUser user, string channelID)
        {
            await UnsubscribeFromChannelAsync(user, channelID, CancellationToken.None);
        }

        public Task UnsubscribeFromChannelAsync(IUser user, string channelID, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Utility methods

        #region Handling chat messages
        private void chatMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {

        }

        private void chatOnErrorReceived(object sender, ErrorEventArgs errorsEventArgs)
        {

        }

        #endregion

        #region User authentication
        private async Task loginIfRequired(IUser user, CancellationToken token)
        {
            using (var writerLock = await _authLocker.WriterLockAsync())
            {
                if (_quickblox.Token != null)
                    return;

                //Get password by computing secret hash of the user id
                var password = getUserPassword(user);

                //Try to authenticate
                var response = await _quickblox.AuthenticationClient.CreateSessionWithLoginAsync(user.ID, password);
                token.ThrowIfCancellationRequested();

                //If the authentication failed it means the user hasn't been created in Quickblox yet
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await signupUser(user, token);
                    //If the signup succeeded (otherwise exception would be thrown) 
                    //we need to create a session with already existing user
                    response = await _quickblox.AuthenticationClient.CreateSessionWithLoginAsync(user.ID, password);
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

        private async Task signupUser(IUser user, CancellationToken token)
        {
            //Create base session, this is required by quickblox API before making any other request
            var sessionResponse = await _quickblox.AuthenticationClient.CreateSessionBaseAsync();
            token.ThrowIfCancellationRequested();

            //Throw exception if session hasn't been created for some reason
            if (sessionResponse.StatusCode != System.Net.HttpStatusCode.Created)
                throw new Exception($"Quickblox user signup failed because Quickblox session creation failed due to unknown error. " +
                                    $"\n HTTP Status code: {sessionResponse.StatusCode}." +
                                    $"Errors: \n {getErrorDesc(sessionResponse.Errors)}");

            //Create user signup request by the current user data
            var userSignUpRequest = new UserSignUpRequest
            {
                User = new UserRequest()
                {
                    Login = user.ID,
                    Email = user.Email,
                    FullName = user.FullName,
                    Password = getUserPassword(user)
                }
            };

            //Try to sign the user up
            var userResponse = await _quickblox.UsersClient.SignUpUserAsync(userSignUpRequest);
            token.ThrowIfCancellationRequested();

            //If the signup failed throw exception
            if (userResponse.StatusCode != System.Net.HttpStatusCode.Created)
                throw new Exception($"Quickblox user signup failed due to unknown error. " +
                                    $"\n HTTP Status code: {userResponse.StatusCode}." +
                                    $"Errors: \n {getErrorDesc(userResponse.Errors)}");
        }


        private async Task<int> getQuickbloxUserId(string userID, CancellationToken token)
        {
            int quickbloxUserId = -1;
            Exception exception = null;
            HttpResponse<RetrieveUsersResponse> response = null;

            try
            {
                //If we have already queried the user id return it from cache
                if (_userIDs.TryGetValue(userID, out quickbloxUserId))
                    return quickbloxUserId;

                //Retrieve the user from quickblox
                //In quickblox we set the login to userID, so we need to query those users which have login == userID
                var retriveUserRequest = new RetrieveUsersRequest();
                var aggregator = new FilterAggregator();
                aggregator.Filters.Add(new RetrieveUserFilter<string>(UserOperator.Eq, () => new Quickblox.Sdk.Modules.UsersModule.Models.User().Login, userID));
                retriveUserRequest.Filter = aggregator;
                response = await _quickblox.UsersClient.RetrieveUsersAsync(retriveUserRequest);
                token.ThrowIfCancellationRequested();

                if (response.StatusCode == HttpStatusCode.OK)
                    quickbloxUserId = response.Result.Items.Select(userResponse => userResponse.User.Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                exception = ex;
            }


            //There are several reasons that quickbloxUserId will be <= 0
            //Exception thrown, response status code wasn't OK, or simply the user hasn't been found
            //Throw an exception in all of those cases, with details about what happened
            if (quickbloxUserId <= 0)
            {
                throw new Exception($"Unable to retrieve Quickblox user for '{userID}'. " +
                                $"\n HTTP Status code: {response?.StatusCode}." +
                                $"Errors: \n {getErrorDesc(response?.Errors)}", exception);
            }

            //Eventually if everything was ok, cached and return the result
            _userIDs[userID] = quickbloxUserId;
            return quickbloxUserId;
        } 
        #endregion

        private string getErrorDesc(Dictionary<string, string[]> errors)
        {
            if (errors == null)
                return "";
            var errorDesc = string.Join("\n", errors.Select(e => $"{e.Key}:  {string.Join(", ", e.Value)}"));
            return errorDesc;
        }

        private string getUserPassword(IUser user)
        {
            if (string.IsNullOrEmpty(user?.ID))
                throw new Exception("Can not generate password with empty user id.");
            var password = Hash.ComputeHash(user.ID.GetBytes()).HashEncode().Substring(0, 40);
            return password;
        }

        #endregion


    }
}

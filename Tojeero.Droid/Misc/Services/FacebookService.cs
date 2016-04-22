﻿using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Java.Lang;
using Newtonsoft.Json;
using Org.Json;
using Tojeero.Core;
using Tojeero.Core.Model;
using Tojeero.Core.Services.Contracts;
using Tojeero.Droid.Toolbox;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace Tojeero.Droid.Services
{
    public class FacebookService : IFacebookService
    {
        #region Private Fields and Properties

        #endregion

        #region Constructors

        #endregion

        #region Properties

        public Activity CurrentActivity { get; set; }

        #endregion

        #region IFacebookService implementation

        public async Task<FacebookUser> GetFacebookToken()
        {
            var result = await getFacebookToken();
            if (result.IsCanceled)
            {
                Mvx.Trace(MvxTraceLevel.Diagnostic, TraceMessages.Auth.FacebookAuthCancelled);
                return null;
            }

            if (result.Exception != null)
            {
                Mvx.Trace(MvxTraceLevel.Error, TraceMessages.Auth.FacebookAuthError, result.Exception);
                return null;
            }

            var token = result.Token;
            Mvx.Trace(MvxTraceLevel.Diagnostic, TraceMessages.Auth.FacebookAuthSuccess, result.Token.Token);

            var fbUser = await getUserData(token);


            return fbUser;
        }

        public void LogOut()
        {
            LoginManager.Instance.LogOut();
        }

        #endregion

        #region Utility Methods

        private async Task<FacebookAuthenticationResult> getFacebookToken()
        {
            var token = AccessToken.CurrentAccessToken;
            if (token != null)
                return new FacebookAuthenticationResult {Token = token};

            var completion = new TaskCompletionSource<FacebookAuthenticationResult>();
            var loginCallback = new FacebookCallback(completion);
            var activity = Xamarin.Forms.Forms.Context as MainActivity;
            LoginManager.Instance.RegisterCallback(activity.CallbackManager, loginCallback);
            LoginManager.Instance.LogInWithReadPermissions(activity, new[] {"public_profile", "email", "user_friends"});
            return await completion.Task;
        }

        private Task<FacebookUser> getUserData(AccessToken token)
        {
            var fbUser = new FacebookUser
            {
                Token = token.Token,
                ExpiryDate = token.Expires.ToDateTime()
            };

            var completion = new TaskCompletionSource<FacebookUser>();
            var callback = new GraphRequestCallback(completion, fbUser);
            var request = GraphRequest.NewMeRequest(token, callback);
            var parameters = new Bundle();
            parameters.PutString("fields", "id, first_name, last_name, email");
            request.Parameters = parameters;
            request.ExecuteAsync();
            return completion.Task;
        }

        #endregion

        #region Private Classes

        private class FacebookAuthenticationResult
        {
            public bool IsCanceled { get; set; }
            public FacebookException Exception { get; set; }
            public AccessToken Token { get; set; }
        }

        private class FacebookCallback : Object, IFacebookCallback
        {
            readonly TaskCompletionSource<FacebookAuthenticationResult> _completion;

            public FacebookCallback(TaskCompletionSource<FacebookAuthenticationResult> completion)
            {
                _completion = completion;
            }

            public void OnCancel()
            {
                _completion.TrySetResult(new FacebookAuthenticationResult {IsCanceled = true});
            }

            public void OnError(FacebookException ex)
            {
                _completion.TrySetResult(new FacebookAuthenticationResult {Exception = ex});
            }

            public void OnSuccess(Object p0)
            {
                _completion.TrySetResult(new FacebookAuthenticationResult {Token = AccessToken.CurrentAccessToken});
            }
        }

        private class GraphRequestCallback : Object, GraphRequest.IGraphJSONObjectCallback
        {
            readonly TaskCompletionSource<FacebookUser> _completion;
            readonly FacebookUser _initialToken;

            public GraphRequestCallback(TaskCompletionSource<FacebookUser> completion, FacebookUser initialToken)
            {
                _completion = completion;
                _initialToken = initialToken;
            }

            public void OnCompleted(JSONObject result, GraphResponse response)
            {
                var fbUser = _initialToken ?? new FacebookUser();
                fbUser.User = JsonConvert.DeserializeObject<User>(response.RawResponse);
                fbUser.User.ProfilePictureUrl =
                    string.Format("https://graph.facebook.com/{0}/picture?width={1}&height={1}", fbUser.User.ID,
                        Constants.FBProfilePicSize);
                _completion.TrySetResult(fbUser);
            }
        }

        #endregion
    }
}
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Facebook.CoreKit;
using Facebook.LoginKit;
using Foundation;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Tojeero.Core;
using Tojeero.Core.Model;
using Tojeero.Core.Services.Contracts;
using Tojeero.iOS.Toolbox;

namespace Tojeero.iOS.Services
{
    public class FacebookService : IFacebookService
    {
        #region Private Fields and Properties

        AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        string _accessToken;

        #endregion

        #region Constructors

        #endregion

        #region IFacebookService implementation

        public async Task<FacebookUser> GetFacebookToken()
        {
            var loginManager = new LoginManager();

            var token = AccessToken.CurrentAccessToken;
            if (token == null)
            {
                var result =
                    await loginManager.LogInWithReadPermissionsAsync(new[] {"public_profile", "email", "user_friends"});
                token = result.Token;
            }
            if (token == null)
                return null;

            var fbUser = await getUserData(token);
            return fbUser;
        }

        public void LogOut()
        {
            var manager = new LoginManager();
            manager.LogOut();
        }

        #endregion

        #region Utility Methods

        private Task<FacebookUser> getUserData(AccessToken token)
        {
            var fbUser = new FacebookUser
            {
                Token = token.TokenString,
                ExpiryDate = token.ExpirationDate.ToDateTime()
            };

            var completion = new TaskCompletionSource<FacebookUser>();

            var request = new GraphRequest("me",
                NSDictionary.FromObjectAndKey(new NSString("id, first_name, last_name, email, location"),
                    new NSString("fields")), "GET");
            request.Start((connection, result, error) =>
            {
                if (error != null)
                {
                    Mvx.Trace(MvxTraceLevel.Error, error.ToString());
                }
                else
                {
                    var data = NSJsonSerialization.Serialize(result as NSDictionary, 0, out error);
                    if (error != null)
                    {
                        Mvx.Trace(MvxTraceLevel.Error, error.ToString());
                    }
                    else
                    {
                        var json = new NSString(data, NSStringEncoding.UTF8).ToString();
                        fbUser.User = JsonConvert.DeserializeObject<User>(json);
                        fbUser.User.ProfilePictureUrl =
                            string.Format("https://graph.facebook.com/{0}/picture?width={1}&height={1}", fbUser.User.ID,
                                Constants.FBProfilePicSize);
                    }
                    completion.TrySetResult(fbUser);
                }
            });
            return completion.Task;
        }

        #endregion
    }
}
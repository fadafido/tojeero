using System;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using UIKit;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core;
using Facebook.LoginKit;
using Facebook.CoreKit;
using Foundation;

namespace Tojeero.iOS
{
	public class FacebookService : IFacebookService
	{
		#region Private Fields and Properties

		AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
		string _accessToken;

		#endregion

		#region Constructors

		public FacebookService()
		{
		}

		#endregion

		#region IFacebookService implementation

		public async Task<FacebookUser> GetFacebookToken()
		{
			var loginManager = new LoginManager();

			var token = Facebook.CoreKit.AccessToken.CurrentAccessToken;
			if (token == null)
			{
				var result = await loginManager.LogInWithReadPermissionsAsync(new string[] {"public_profile", "email", "user_friends"});
				token = result.Token;
			}
			if (token == null)
				return null;
			
			var fbUser = await getUserData(token);
			return fbUser;
		}

		public void LogOut()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Utility Methods

		private Task<FacebookUser> getUserData(Facebook.CoreKit.AccessToken token)
		{
			var fbUser = new FacebookUser()
				{
					Token = token.TokenString,
					ExpiryDate = token.ExpirationDate.ToDateTime()
				};

			var completion = new TaskCompletionSource<FacebookUser>();

			var request = new GraphRequest("me", NSDictionary.FromObjectAndKey(new NSString("id, first_name, last_name, email, location"), new NSString("fields")), "GET");
			request.Start((connection, result, error) =>
				{
					completion.TrySetResult(fbUser);
				});
			return completion.Task;
		}

		#endregion
	}
}


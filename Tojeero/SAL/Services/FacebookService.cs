using System;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using UIKit;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core;
using Facebook.LoginKit;

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

		public async Task<string> GetFacebookToken()
		{
			var loginManager = new LoginManager();
			var result = await loginManager.LogInWithReadPermissionsAsync(new string[] {"public_profile", "email", "user_friends"});
			return result != null && result.Token != null ? result.Token.TokenString : null;
		}

		public void LogOut()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Utility Methods


		#endregion

		/*Get Facebook token using FAcebook iOS Sdk 3.22.0
		 * public Task<string> GetFacebookToken()
		{
//			var taskCompletion = new TaskCompletionSource<string>();
//			var loginManager = new LoginManager();
//			var result = await loginManager.LogInWithReadPermissionsAsync(new string[] {"public_profile", "email", "user_friends"});
//			return result != null && result.Token != null ? result.Token.TokenString : null;

			var taskCompletion = new TaskCompletionSource<string>();

			if (FBSession.ActiveSession.State == FBSessionState.Open ||
				FBSession.ActiveSession.State == FBSessionState.OpenTokenExtended)
			{
				taskCompletion.TrySetResult(FBSession.ActiveSession.AccessTokenData.AccessToken);
			}
			else
			{
				FBSessionStateHandler completion = async delegate(FBSession session, FBSessionState status, Foundation.NSError error)
				{
					if (status == FBSessionState.Open)
					{
						taskCompletion.TrySetResult(FBSession.ActiveSession.AccessTokenData.AccessToken);
					}
					else
					{
						taskCompletion.TrySetResult(null);
					}
				};
				
				try
				{
					if (FBSession.ActiveSession.State == FBSessionState.CreatedTokenLoaded) 
					{
						FBSession.ActiveSession.Open(FBSessionLoginBehavior.UseSystemAccountIfPresent, completion);
					} 
					else 
					{
						this.clearUserInfo();
						FBSession session = new FBSession(new String[] { "public_profile", "email", "user_friends" });
						FBSession.ActiveSession = session;
						session.Open(FBSessionLoginBehavior.UseSystemAccountIfPresent, completion);
					}
				}
				catch(Exception ex)
				{
					Mvx.Trace(ex.ToString());
					taskCompletion.TrySetResult(null);
				}
			}

			return taskCompletion.Task;
		}

		private void clearUserInfo()
		{
			if (FBSession.ActiveSession != null)
			{
				FBSession.ActiveSession.CloseAndClearTokenInformation();
				FBSession.RenewSystemCredentials((result, error) =>
					{
					});
			}
		}
		*/
	}
}


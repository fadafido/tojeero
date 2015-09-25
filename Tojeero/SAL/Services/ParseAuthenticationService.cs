using System;
using Tojeero.Core;
using System.Threading.Tasks;
using Parse;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;


namespace Tojeero.Core.Services
{
	public class ParseAuthenticationService : IAuthenticationService
	{
		private readonly IFacebookService _facebookService;

		public ParseAuthenticationService(IFacebookService facebookService)
		{
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
		}

		private SessionState _state;
		public SessionState State
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public event EventHandler<EventArgs<SessionState>> SessionStateChanged;



		public Task LogOut()
		{
			throw new NotImplementedException();
		}

		public async Task<User> LogInWithFacebook()
		{
			var session = await ParseSession.GetCurrentSessionAsync();

			var fbUser = await _facebookService.GetFacebookToken();
			if(fbUser == null)
				return null;

			try
			{
				var parseUser = await ParseFacebookUtils.LogInAsync(fbUser.ID, fbUser.Token, fbUser.ExpiryDate);
				parseUser.Email = fbUser.Email;
				parseUser["firstName"] = fbUser.FirstName;
				parseUser["lastName"] = fbUser.FirstName;
				parseUser["profilePictureUri"] = fbUser.ProfilePictureUri;
				await parseUser.SaveAsync();
					
				return null;
			}
			catch(Exception ex)
			{
				Mvx.Trace(MvxTraceLevel.Error, ex.ToString());
				return null;
			}

		}
			
		#endregion
	}
}


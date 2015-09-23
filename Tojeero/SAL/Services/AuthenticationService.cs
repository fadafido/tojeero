using System;
using Tojeero.Core;
namespace Tojeero.Core.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		public AuthenticationService()
		{
		}

		#region IAuthenticationService implementation

		public event EventHandler<EventArgs<SessionState>> SessionStateChanged;

		public System.Threading.Tasks.Task<AuthenticationResult<User>> LogIn(string username, string password)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<AuthenticationResult<User>> SignUp(string firstName, string lastName, string username, string password)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<AuthenticationResult<User>> RecoverSession()
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task LogOut()
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<AuthenticationResult<User>> LogInWithFacebook(string facebookToken)
		{
			throw new NotImplementedException();
		}

		public System.Threading.Tasks.Task<AuthenticationResult<User>> SignUpWithFacebook(string facebookToken)
		{
			throw new NotImplementedException();
		}

		public User CurrentUser
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SessionState State
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}


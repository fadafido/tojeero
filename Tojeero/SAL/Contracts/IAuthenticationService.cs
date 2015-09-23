using System;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IAuthenticationService 
	{		
		User CurrentUser { get; }
		SessionState State { get; }
		event EventHandler<EventArgs<SessionState>> SessionStateChanged;

		Task<AuthenticationResult<User>> LogIn(string username, string password);
		Task<AuthenticationResult<User>> SignUp(string firstName, string lastName, string username, string password);
		Task<AuthenticationResult<User>> RecoverSession();
		Task LogOut();
		Task<AuthenticationResult<User>> LogInWithFacebook(string facebookToken);
		Task<AuthenticationResult<User>> SignUpWithFacebook(string facebookToken);
	}
}


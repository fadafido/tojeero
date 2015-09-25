using System;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IAuthenticationService 
	{		
		User CurrentUser { get; }
		SessionState State { get; }
		event EventHandler<EventArgs<SessionState>> SessionStateChanged;

		Task LogOut();
		Task<User> LogInWithFacebook();
	}
}


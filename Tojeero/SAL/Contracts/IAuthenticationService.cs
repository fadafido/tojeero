using System;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core.Services
{
	public interface IAuthenticationService 
	{		
		User CurrentUser { get; }
		SessionState State { get; }

		Task LogOut();
		Task RestoreSavedSession();
		Task<User> LogInWithFacebook();
		Task UpdateUserDetails(User user, CancellationToken token);
	}
}


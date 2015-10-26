using System;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core.Services
{
	public interface IAuthenticationService 
	{		
		IUser CurrentUser { get; }
		SessionState State { get; }

		Task LogOut();
		Task RestoreSavedSession();
		Task<IUser> LogInWithFacebook();
		Task UpdateUserDetails(IUser user, CancellationToken token);
	}
}


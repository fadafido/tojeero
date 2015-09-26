using System;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IAuthenticationService 
	{		
		User CurrentUser { get; }
		SessionState State { get; }

		void LogOut();
		Task RestoreSavedSession();
		Task<User> LogInWithFacebook();
	}
}


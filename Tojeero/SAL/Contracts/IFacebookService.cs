using System;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IFacebookService
	{
		Task<FacebookUser> GetFacebookToken();
		void LogOut();
	}
}


using System.Threading.Tasks;
using Tojeero.Core.Model;

namespace Tojeero.Core.Services.Contracts
{
	public interface IFacebookService
	{
		Task<FacebookUser> GetFacebookToken();
		void LogOut();
	}
}


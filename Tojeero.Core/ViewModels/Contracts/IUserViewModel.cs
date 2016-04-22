using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Contracts
{
	public interface IUserViewModel
	{
		IUser CurrentUser { get; set; }
		bool IsLoggedIn { get; }
		bool ShouldSubscribeToSessionChange { get; set; }
	}
}


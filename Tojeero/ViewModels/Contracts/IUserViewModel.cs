using System;
using Tojeero.Core;

namespace Tojeero.Core.ViewModels
{
	public interface IUserViewModel
	{
		IUser CurrentUser { get; set; }
		bool IsLoggedIn { get; }
		bool ShouldSubscribeToSessionChange { get; set; }
	}
}


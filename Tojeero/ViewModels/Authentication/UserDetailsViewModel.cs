using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Core.ViewModels
{
	public class UserDetailsViewModel : BaseUserViewModel
	{
		#region Constructors

		public UserDetailsViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
		}

		#endregion

	}
}


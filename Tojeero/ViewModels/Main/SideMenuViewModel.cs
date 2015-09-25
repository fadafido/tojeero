using System;
using Tojeero.Core.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class SideMenuViewModel : LoadableViewModel
	{
		#region Private Fields and Properties

		private readonly IAuthenticationService _authService;

		#endregion

		#region Constructors

		public SideMenuViewModel(IAuthenticationService authService)
		{
			_authService = authService;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> ShowUserDetails;

		private bool _isLoggingIn;
		public bool IsLoggingIn
		{ 
			get
			{
				return _isLoggingIn; 
			}
			set
			{
				_isLoggingIn = value; 
				RaisePropertyChanged(() => IsLoggingIn); 
			}
		}

		#endregion

		#region UI Strings


		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loginCommand;
		public System.Windows.Input.ICommand LoginCommand
		{
			get
			{
				_loginCommand = _loginCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await logIn();
				});
				return _loginCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task logIn()
		{
			this.IsLoggingIn = true;
			string failureMessage = "";
			var result = await this._authService.LogInWithFacebook(); 
			ShowUserDetails.Fire(this, new EventArgs());
			this.IsLoggingIn = false;
		}

		#endregion
	}
}


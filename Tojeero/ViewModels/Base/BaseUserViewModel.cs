using System;
using System.Threading.Tasks;
using Tojeero.Core.Services;

namespace Tojeero.Core.ViewModels
{
	public class BaseUserViewModel : LoadableViewModel
	{
		private readonly IAuthenticationService _authService;
		#region Constructors

		public BaseUserViewModel(IAuthenticationService authService)
			: base()
		{
			_authService = authService;
		}

		#endregion

		#region Properties

		private User _currentUser;

		public User CurrentUser
		{ 
			get
			{
				return _currentUser; 
			}
			set
			{
				_currentUser = value; 
				RaisePropertyChanged(() => CurrentUser); 
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _logOutCommand;

		public System.Windows.Input.ICommand LogOutCommand
		{
			get
			{
				_logOutCommand = _logOutCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(DoLogOut);
				return _logOutCommand;
			}
		}

		private async void DoLogOut()
		{
			await logOut();
		}

		#endregion

		#region Protected API

		protected async Task<bool> RecoverUserSession()
		{
			this.StartLoading();
			var result = await _authService.RecoverSession();
			string failureMessage = null;
			switch (result.ResultCode)
			{
				case AuthenticationResultCode.Successful:
					break;
				case AuthenticationResultCode.TokenNotFound:
					failureMessage = "You are not logged in. Please log in and try again.";
					break;
				case AuthenticationResultCode.TokenExpired:
					failureMessage = "You session has expired. Please log in and try again.";
					break;
				case AuthenticationResultCode.WebException:
					failureMessage = "Failed due to network issue. Please make sure you are connected to internet and try again.";
					break;
				default:
					failureMessage = "Failed due to unknown issue. Please try again later.";
					break;
			}
			if (failureMessage != null)
				return false;
			return true;
		}

		#endregion

		#region Utility Methods

		private async Task logOut()
		{
			this.StartLoading("Logging out...");
			await _authService.LogOut();
			this.StopLoading();
		}

		#endregion
	}
}


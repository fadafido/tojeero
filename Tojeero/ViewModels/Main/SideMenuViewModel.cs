using System;
using Tojeero.Core.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Xamarin.Forms;
using Tojeero.Forms;

namespace Tojeero.Core.ViewModels
{
	public class SideMenuViewModel : BaseUserDetailsViewModel
	{
		#region Private Fields and Properties

		private readonly IAuthenticationService _authService;

		#endregion

		#region Constructors

		public SideMenuViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			:base(authService, messenger)
		{
			_authService = authService;
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> ShowUserDetails;

		#endregion

		#region UI Strings


		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loginCommand;
		public System.Windows.Input.ICommand LoginCommand
		{
			get
			{
				_loginCommand = _loginCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await logIn(), () => !IsLoading);
				return _loginCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _logoutCommand;
		public System.Windows.Input.ICommand LogoutCommand
		{
			get
			{
				_logoutCommand = _logoutCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => logOut(), () => !IsLoading);
				return _logoutCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task logIn()
		{
			this.IsLoading = true;
			var result = await this._authService.LogInWithFacebook(); 
			this.IsLoading = false;
		}

		private async Task logOut()
		{
			this._authService.LogOut();
		}
			
		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsLoading")
			{
				_loginCommand.RaiseCanExecuteChanged();
				_logoutCommand.RaiseCanExecuteChanged();
			}
		}
		#endregion
	}
}


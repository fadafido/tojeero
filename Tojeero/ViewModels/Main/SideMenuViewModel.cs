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
		#region Constructors

		public SideMenuViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			:base(authService, messenger)
		{
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs<bool>> ShowProfileSettings;

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
				_logoutCommand = _logoutCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await logOut(), () => !IsLoading);
				return _logoutCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showProfileSettingsCommand;
		public System.Windows.Input.ICommand ShowProfileSettingsCommand
		{
			get
			{
				_showProfileSettingsCommand = _showProfileSettingsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => this.ShowProfileSettings.Fire(this, new EventArgs<bool>(false)));
				return _showProfileSettingsCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task logIn()
		{
			this.IsLoading = true;
			var result = await this._authService.LogInWithFacebook(); 
			if (result != null && !result.IsProfileSubmitted)
			{
				this.ShowProfileSettings.Fire(this, new EventArgs<bool>(true));
			}
			this.IsLoading = false;
		}

		private async Task logOut()
		{
			this.IsLoading = true;
			await this._authService.LogOut();
			this.IsLoading = false;
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


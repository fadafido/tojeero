using System;
using Tojeero.Core.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tojeero.Core.ViewModels
{
	public class LogInViewModel : LoadableViewModel
	{
		#region Private Fields and Properties

		private readonly IAuthenticationService _authService;

		#endregion

		#region Constructors

		public LogInViewModel(IAuthenticationService authService)
		{
			_authService = authService;
		}

		#endregion

		#region Properties

		private string _username = "gagik.kyurkchyan@gmail.com";
		public string Username
		{ 
			get
			{
				return _username; 
			}
			set
			{
				_username = value; 
				RaisePropertyChanged(() => Username); 
				validateUsername();
			}
		}

		private string _usernameError;
		public string UsernameError
		{ 
			get
			{
				return _usernameError; 
			}
			set
			{
				_usernameError = value; 
				RaisePropertyChanged(() => UsernameError); 
			}
		}

		private string _password="Jesus%IsLord";
		public string Password
		{ 
			get
			{
				return _password; 
			}
			set
			{
				_password = value; 
				RaisePropertyChanged(() => Password); 
				validatePassword();
			}
		}

		private string _passwordError;
		public string PasswordError
		{ 
			get
			{
				return _passwordError; 
			}
			set
			{
				_passwordError = value; 
				RaisePropertyChanged(() => PasswordError); 
			}
		}


		#endregion

		#region UI Strings

		public string UsernameLabel
		{
			get
			{
				return "Username";
			}
		}

		public string UsernamePlaceholder
		{
			get
			{
				return "someone@mail.com";
			}
		}

		public string PasswordLabel
		{
			get
			{
				return "Password";
			}
		}

		public string PasswordPlaceholder
		{
			get
			{
				return "Password";
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loginCommand;
		public System.Windows.Input.ICommand LoginCommand
		{
			get
			{
				_loginCommand = _loginCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(DoLogin, () => false);
				return _loginCommand;
			}
		}

		private async void DoLogin()
		{
			if (!validate())
				return;
			//do action
			await logIn();
		}

		#endregion

		#region Utility Methods

		private async Task logIn()
		{
			this.StartLoading("Loggin In...");
			string failureMessage = "";
			var result = await this._authService.LogIn(this.Username, this.Password); 
			switch (result.ResultCode)
			{
				case AuthenticationResultCode.Successful:
					{
						this.ShowViewModel<UserDetailsViewModel>();
					}
					break;
				case AuthenticationResultCode.WrongCredentials:
					{
						failureMessage = "Invalid user name and password combination.";
					}
					break;
				case AuthenticationResultCode.WebException:
					{
						failureMessage = "Authentication failed due to network issue. Please make sure you are connected to internet and try again.";
					}
					break;
				default:
					failureMessage = "Failed to log in. Please try again later.";
					break;
			}
			this.StopLoading(failureMessage);
		}

		private bool validate()
		{
			var result = validateUsername() && validatePassword();
			return result;
		}

		private bool validateUsername()
		{
			bool isEmail = Regex.IsMatch(Username, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
			if (!isEmail)
			{
				UsernameError = "Please enter valid email address.";
				return false;
			}
			UsernameError = "";
			return true;
		}

		private bool validatePassword()
		{
			if (string.IsNullOrEmpty(Password))
			{
				PasswordError = "Please enter non empty password.";
				return false;
			}
			PasswordError = "";
			return true;
		}

		#endregion
	}
}


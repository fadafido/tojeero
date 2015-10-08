using System;
using Tojeero.Core.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Xamarin.Forms;
using Tojeero.Forms;
using Cirrious.CrossCore;

namespace Tojeero.Core.ViewModels
{
	public class SideMenuViewModel : BaseUserViewModel
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
				_loginCommand = _loginCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await logIn(), () => CanExecuteLoginCommand);
				return _loginCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _logoutCommand;
		public System.Windows.Input.ICommand LogoutCommand
		{
			get
			{
				_logoutCommand = _logoutCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await logOut(), () => !this.IsLoading);
				return _logoutCommand;
			}
		}

		public bool CanExecuteLoginCommand
		{
			get 
			{
				return !IsLoading && IsNetworkAvailable;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showProfileSettingsCommand;
		public System.Windows.Input.ICommand ShowProfileSettingsCommand
		{
			get
			{
				_showProfileSettingsCommand = _showProfileSettingsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => 
					{
						var language = Settings.Language != null ? Settings.Language.Value : LanguageCode.English;
						Mvx.Resolve<ILocalizationService>().SetLanguage(language == LanguageCode.English ? LanguageCode.Arabic : LanguageCode.English);
						this.ShowProfileSettings.Fire(this, new EventArgs<bool>(false));
					});
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
			if(e.PropertyName == IsLoadingPropertyName || e.PropertyName == IsNetworkAvailablePropertyName)
			{
				RaisePropertyChanged(() => CanExecuteLoginCommand);
			}
		}
		#endregion
	}
}


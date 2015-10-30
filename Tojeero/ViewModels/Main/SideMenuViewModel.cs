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
using System.Globalization;
using Tojeero.Core.Messages;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class SideMenuViewModel : BaseUserViewModel
	{
		#region Private fields and properties

		private readonly ILocalizationService _localizationService;
		MvxSubscriptionToken _token;
		#endregion

		#region Constructors

		public SideMenuViewModel(IAuthenticationService authService, IMvxMessenger messenger, ILocalizationService localizationService )
			:base(authService, messenger)
		{
			this._localizationService = localizationService;
			_token = messenger.Subscribe<LanguageChangedMessage>((message) =>
				{
					RaisePropertyChanged(() => NewLanguage);
				});
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public Action<bool> ShowProfileSettings;
		public Action ShowFavorites;
		public Action<string> ShowLanguageChangeWarning;

		public LanguageCode NewLanguage
		{ 
			get
			{
				return _localizationService.Language == LanguageCode.Arabic ? LanguageCode.English : LanguageCode.Arabic; 
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
						this.ShowProfileSettings.Fire(false);
					});
				return _showProfileSettingsCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _changeLanguageCommand;
		public System.Windows.Input.ICommand ChangeLanguageCommand
		{
			get
			{
				_changeLanguageCommand = _changeLanguageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						Mvx.Resolve<ILocalizationService>().SetLanguage(this.NewLanguage);
						ShowLanguageChangeWarning.Fire(AppResources.MessageLanguageChangeWarning);
					});
				return _changeLanguageCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showFavoritesCommand;

		public System.Windows.Input.ICommand ShowFavoritesCommand
		{
			get
			{
				_showFavoritesCommand = _showFavoritesCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					this.ShowFavorites.Fire();
				});
				return _showFavoritesCommand;
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
				this.ShowProfileSettings.Fire(true);
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
			if(e.PropertyName == IsLoadingProperty || e.PropertyName == IsNetworkAvailableProperty)
			{
				RaisePropertyChanged(() => CanExecuteLoginCommand);
			}
		}
		#endregion
	}
}


﻿using System;
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
using System.Threading;

namespace Tojeero.Core.ViewModels
{
	public class SideMenuViewModel : BaseUserStoreViewModel
	{
		#region Private fields and properties

		private readonly ILocalizationService _localizationService;
		private readonly MvxSubscriptionToken _languageChangeToken;

		#endregion

		#region Constructors

		public SideMenuViewModel(IAuthenticationService authService, IMvxMessenger messenger, ILocalizationService localizationService )
			:base(authService, messenger)
		{
			this.ShouldSubscribeToSessionChange = true;
			this._localizationService = localizationService;
			_languageChangeToken = messenger.Subscribe<LanguageChangedMessage>((message) =>
				{
					RaisePropertyChanged(() => NewLanguage);
				});

			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public Action<bool> ShowProfileSettings { get; set; }
		public Action<string> ShowLanguageChangeWarning { get; set; }
		public Action ShowTermsAction { get; set; }

		public LanguageCode NewLanguage
		{ 
			get
			{
				return _localizationService.Language == LanguageCode.Arabic ? LanguageCode.English : LanguageCode.Arabic; 
			}
		}
			
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

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showTermsCommand;

		public System.Windows.Input.ICommand ShowTermsCommand
		{
			get
			{
				_showTermsCommand = _showTermsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>{
					ShowTermsAction.Fire();
				});
				return _showTermsCommand;
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


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
	public class SideMenuViewModel : BaseUserViewModel
	{
		#region Private fields and properties

		private readonly ILocalizationService _localizationService;
		private readonly MvxSubscriptionToken _languageChangeToken;
		private readonly MvxSubscriptionToken _storeChangeToken;
		private bool _isLoggedIn;

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
			_storeChangeToken = messenger.Subscribe<StoreChangedMessage>((message) =>
				{
					//If the changed store is related to current user then refetch the user store to reflect the change
					if(message.Store != null && this.CurrentUser != null &&
						message.Store.OwnerID == this.CurrentUser.ID)
					{
						reloadUserStore();
					}
				});
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public Action<IStore> ShowSaveStoreAction;
		public Action<bool> ShowProfileSettings;
		public Action<string> ShowLanguageChangeWarning;

		public LanguageCode NewLanguage
		{ 
			get
			{
				return _localizationService.Language == LanguageCode.Arabic ? LanguageCode.English : LanguageCode.Arabic; 
			}
		}

		private bool _isLoadingUserStore;
		public static string IsLoadingUserStoreProperty = "IsLoadingUserStore";

		public bool IsLoadingUserStore
		{ 
			get
			{
				return _isLoadingUserStore; 
			}
			set
			{
				_isLoadingUserStore = value; 
				RaisePropertyChanged(() => IsLoadingUserStore); 
			}
		}

		private bool _isLoadingUserStoreFailed;
		public static string IsLoadingUserStoreFailedProperty = "IsLoadingUserStoreFailed";
		public bool IsLoadingUserStoreFailed
		{ 
			get
			{
				return _isLoadingUserStoreFailed; 
			}
			set
			{
				_isLoadingUserStoreFailed = value; 
				RaisePropertyChanged(() => IsLoadingUserStoreFailed); 
			}
		}

		private IStore _userStore;
		public static string UserStoreProperty = "UserStore";
		public IStore UserStore
		{ 
			get
			{
				return _userStore; 
			}
			set
			{
				_userStore = value; 
				RaisePropertyChanged(() => UserStore); 
				RaisePropertyChanged(() => ShowSaveStoreTitle);
			}
		}

		public string ShowSaveStoreTitle
		{
			get
			{
				string title;

				if (this.UserStore != null)
				{
					title = AppResources.ButtonMyStore;
				}
				else
				{
					title = AppResources.ButtonCreateStore;
				}
				return title;
			}
		}

		public bool IsShowSaveStoreVisible
		{
			get
			{
				return !IsLoadingUserStore && !IsLoadingUserStoreFailed;
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

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showSaveStoreCommand;

		public System.Windows.Input.ICommand ShowSaveStoreCommand
		{
			get
			{
				_showSaveStoreCommand = _showSaveStoreCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					this.ShowSaveStoreAction.Fire(this.UserStore);
				});
				return _showSaveStoreCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadUserStoreCommand;

		public System.Windows.Input.ICommand LoadUserStoreCommand
		{
			get
			{
				_loadUserStoreCommand = _loadUserStoreCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await loadUserStore();
					}, () => CanExecuteLoadUserStoreCommand);
				return _loadUserStoreCommand;
			}
		}
			
		public bool CanExecuteLoadUserStoreCommand
		{
			get
			{
				return this.IsLoggedIn && this.IsNetworkAvailable && !IsLoadingUserStore && this.UserStore == null;
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

		private async Task loadUserStore()
		{
			this.IsLoadingUserStore = true;
			this.IsLoadingUserStoreFailed = false;
			try
			{
				if(this.CurrentUser.DefaultStore == null)
					await this.CurrentUser.LoadDefaultStore();
				this.UserStore = this.CurrentUser.DefaultStore;
			}
			catch(OperationCanceledException ex)
			{
				this.IsLoadingUserStoreFailed = true;
				Tools.Logger.Log(ex, LoggingLevel.Debug);
			}
			catch (Exception ex)
			{
				this.IsLoadingUserStoreFailed = true;
				Tools.Logger.Log(ex, LoggingLevel.Error, true);
			}
			this.IsLoadingUserStore = false;
		}
			
		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{			
			if(e.PropertyName == IsLoadingProperty || e.PropertyName == IsNetworkAvailableProperty)
			{
				RaisePropertyChanged(() => CanExecuteLoginCommand);
			}

			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty || 
				e.PropertyName == IsLoadingUserStoreProperty || e.PropertyName == UserStoreProperty)
			{
				RaisePropertyChanged(() => CanExecuteLoadUserStoreCommand);
			}

			if (e.PropertyName == IsLoadingUserStoreProperty || e.PropertyName == IsLoadingUserStoreFailedProperty)
			{
				RaisePropertyChanged(() => IsShowSaveStoreVisible);
			}

			//If the logged in property has changed we need to reload user store.
			if (e.PropertyName == IsLoggedInProperty && _isLoggedIn != this.IsLoggedIn)
			{
				_isLoggedIn = this.IsLoggedIn;
				reloadUserStore();
			}
		}


		void reloadUserStore()
		{
			this.UserStore = null;
			LoadUserStoreCommand.Execute(null);
		}

		#endregion
	}
}


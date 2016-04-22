using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Logging;
using Tojeero.Core.Messages;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels.Common
{
	public class BaseUserStoreViewModel : BaseUserViewModel
	{
		#region Private fields and properties

		private readonly MvxSubscriptionToken _languageChangeToken;
		private readonly MvxSubscriptionToken _storeChangeToken;
		private bool _isLoggedIn;

		#endregion

		#region Constructors

		public BaseUserStoreViewModel(IAuthenticationService authService, IMvxMessenger messenger )
			:base(authService, messenger)
		{
			this.ShouldSubscribeToSessionChange = true;

			_storeChangeToken = messenger.SubscribeOnMainThread<StoreChangedMessage>((message) =>
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

		public Action<IStore> ShowSaveStoreAction { get; set; }
		public Action<IStore> DidLoadUserStoreAction { get; set; }
		public Action IsLoadingStoreAction { get; set; }


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

		private async Task loadUserStore()
		{
			this.IsLoadingUserStore = true;
			this.IsLoadingUserStoreFailed = false;
			try
			{
				if(this.CurrentUser.DefaultStore == null)
					await this.CurrentUser.LoadDefaultStore();
				this.UserStore = this.CurrentUser.DefaultStore;
				DidLoadUserStoreAction.Fire(this.UserStore);
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
			this.IsLoadingStoreAction.Fire();
			LoadUserStoreCommand.Execute(null);
		}

		#endregion
	}
}


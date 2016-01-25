using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class StoreViewModel : BaseUserViewModel, ISocialViewModel
	{
		#region Private fields and properties

		AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public StoreViewModel(IStore store = null)
			: base(Mvx.Resolve<IAuthenticationService>(), Mvx.Resolve<IMvxMessenger>())
		{
			this.Store = store;
			this.PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public static string StoreProperty = "Store";
		private IStore _store;

		public virtual IStore Store
		{ 
			get
			{
				return _store; 
			}
			set
			{
				if (_store != null)
					_store.PropertyChanged -= propertyChanged;
				_store = value; 
				if (_store != null)
					_store.PropertyChanged += propertyChanged;
				RaisePropertyChanged(() => Store); 
				this.LoadFavoriteCommand.Execute(null);
			}
		}

		private IFavorite _favorite;

		public IFavorite Favorite
		{ 
			get
			{
				return _favorite; 
			}
			set
			{
				_favorite = value; 
				RaisePropertyChanged(() => Favorite); 
				RaisePropertyChanged(() => IsFavoriteToggleVisible); 
			}
		}

		public bool IsFavoriteToggleVisible
		{
			get
			{
				return this.Favorite != null;
			}
		}

		public virtual string StatusWarning
		{
			get
			{
				string warning = null;
				if (this.Store != null)
				{
					if (this.Store.IsBlocked)
						warning = AppResources.MessageStoreBlocked;
				}
				return warning;
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFavoriteCommand;

		public System.Windows.Input.ICommand LoadFavoriteCommand
		{
			get
			{
				_loadFavoriteCommand = _loadFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await loadFavorite();
					}, () => CanExecuteLoadFavoriteCommand);
				return _loadFavoriteCommand;
			}
		}

		public static string CanExecuteLoadFavoriteCommandProperty = "CanExecuteLoadFavoriteCommand";
		public bool CanExecuteLoadFavoriteCommand
		{
			get
			{
				return this.Store != null && this.Store.ID != null && this.Favorite == null && this.IsNetworkAvailable && this.IsLoggedIn;
			}
		}


		private Cirrious.MvvmCross.ViewModels.MvxCommand _toggleFavoriteCommand;

		public System.Windows.Input.ICommand ToggleFavoriteCommand
		{
			get
			{
				_toggleFavoriteCommand = _toggleFavoriteCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await toggleFavorite();
					}, () => CanExecuteToggleFavoriteCommand);
				return _toggleFavoriteCommand;
			}
		}

		public static string CanExecuteToggleFavoriteCommandProperty = "CanExecuteToggleFavoriteCommand";
		public bool CanExecuteToggleFavoriteCommand
		{
			get
			{
				return this.Store != null && this.Store.ID != null && this.Favorite != null && this.IsNetworkAvailable && !this.IsLoading && this.IsLoggedIn;
			}
		}

		#endregion

		#region Utility methods

		protected async Task loadFavorite()
		{
			if (!CanExecuteLoadFavoriteCommand)
				return;
			StartLoading();
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					this.Favorite = await _authService.CurrentUser.GetStoreFavorite(this.Store.ID);
				}
				catch (Exception ex)
				{
					failureMessage = "Failed to load favorite.";
					Tools.Logger.Log(ex, "Failed to load favorite for store with ID '{0}'", LoggingLevel.Error, true, this.Store.ID);
				}
			}
			StopLoading(failureMessage);
		}

		private async Task toggleFavorite()
		{
			if (!CanExecuteToggleFavoriteCommand)
				return;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					if(this.Favorite.IsFavorite)
					{
						await _authService.CurrentUser.RemoveStoreFromFavorites(this.Store.ID);
					}
					else
					{
						await _authService.CurrentUser.AddStoreToFavorites(this.Store.ID);
					}
				}
				catch (Exception ex)
				{
					Tools.Logger.Log(ex, "Failed to load favorite for store with ID '{0}'", LoggingLevel.Error, true, this.Store.ID);
				}
			}
		}

		private void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
				e.PropertyName == "Favorite" || e.PropertyName == IsLoadingProperty || e.PropertyName == "")
			{				
				this.RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
			}

			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
				e.PropertyName == StoreProperty || e.PropertyName == "")
			{
				this.RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
			}		

			if (e.PropertyName == CanExecuteLoadFavoriteCommandProperty || e.PropertyName == "")
			{
				this.LoadFavoriteCommand.Execute(null);
			}

			if (e.PropertyName == "Status" || e.PropertyName == "Mode" || e.PropertyName == "")
			{
				RaisePropertyChanged(() => StatusWarning);
			}

			//If the user state has changed to logged off then we need to clean the favorite state
			if ((e.PropertyName == IsLoggedInProperty || e.PropertyName == "") && !this.IsLoggedIn && this.Store != null)
			{
				this.Favorite = null;
			}
		}

		#endregion
	}
}


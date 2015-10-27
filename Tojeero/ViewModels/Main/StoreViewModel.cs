﻿using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Tojeero.Core.ViewModels
{
	public class StoreViewModel : BaseUserViewModel
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

		public IStore Store
		{ 
			get
			{
				return _store; 
			}
			set
			{
				_store = value; 
				RaisePropertyChanged(() => Store); 
				this.IsFavorite = false;
				this.IsFavoriteLoaded = false;
				this.LoadFavoriteCommand.Execute(null);
			}
		}

		public static string IsFavoriteProperty = "IsFavorite";
		private bool _isFavorite;

		public bool IsFavorite
		{ 
			get
			{
				return _isFavorite; 
			}
			set
			{
				_isFavorite = value; 
				RaisePropertyChanged(() => IsFavorite); 
			}
		}

		public static string IsFavoriteLoadedProperty = "IsFavoriteLoaded";
		private bool _isFavoriteLoaded;

		public bool IsFavoriteLoaded
		{ 
			get
			{
				return _isFavoriteLoaded; 
			}
			set
			{
				_isFavoriteLoaded = value; 
				RaisePropertyChanged(() => IsFavoriteLoaded); 
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
				return this.Store != null && this.Store.ID != null && this.IsNetworkAvailable && this.IsLoggedIn;
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
				return this.IsFavoriteLoaded && this.IsNetworkAvailable && !this.IsLoading && this.IsLoggedIn;
			}
		}

		#endregion

		#region Utility methods

		private static Random favRand = new Random();

		private async Task loadFavorite()
		{
			StartLoading();
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					if (this.IsFavoriteLoaded)
						return;
					this.IsFavorite = await _authService.CurrentUser.IsStoreFavorite(this.Store.ID);
					this.IsFavoriteLoaded = true;
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
			StartLoading();
			string failureMessage = null;
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					if(this.IsFavorite)
					{
						await _authService.CurrentUser.RemoveStoreFromFavorites(this.Store.ID);
					}
					else
					{
						await _authService.CurrentUser.AddStoreToFavorites(this.Store.ID);
					}
					this.IsFavorite = !this.IsFavorite;
				}
				catch (Exception ex)
				{
					failureMessage = "Failed to toggle favorite.";
					Tools.Logger.Log(ex, "Failed to load favorite for store with ID '{0}'", LoggingLevel.Error, true, this.Store.ID);
				}
			}
			StopLoading(failureMessage);
		}

		private void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
			    e.PropertyName == IsFavoriteLoadedProperty || e.PropertyName == IsLoadingProperty)
			{				
				this.RaisePropertyChanged(() => CanExecuteToggleFavoriteCommand);
			}

			if (e.PropertyName == IsLoggedInProperty || e.PropertyName == IsNetworkAvailableProperty ||
			         e.PropertyName == StoreProperty)
			{
				this.RaisePropertyChanged(() => CanExecuteLoadFavoriteCommand);
			}		

			if (e.PropertyName == CanExecuteLoadFavoriteCommandProperty)
			{
				this.LoadFavoriteCommand.Execute(null);
			}

			//If the user state has changed to logged off then we need to clean the favorite state
			if (e.PropertyName == IsLoggedInProperty && !this.IsLoggedIn)
			{
				this.IsFavorite = false;
				this.IsFavoriteLoaded = false;
			}
		}

		#endregion
	}
}

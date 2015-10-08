using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class BaseCollectionViewModel<T> : LoadableNetworkViewModel 
		where T : IModelEntity
	{
		#region Private fields and properties

		private enum Commands
		{
			Unknown,
			LoadFirstPage,
			LoadNextPage,
			Reload
		}

		QueryDelegate<T> _query;
		Func<Task> _clearCacheTask;
		private int _pageSize;
		private bool _isFirstPageLoaded;
		private Commands _lastExecutedCommand;

		#endregion


		#region Constructors

		public BaseCollectionViewModel(QueryDelegate<T> query, Func<Task> clearCacheTask, int pageSize = 10)
			: base()
		{			
			_pageSize = pageSize;
			_query = query;
			_clearCacheTask = clearCacheTask;
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> LoadingNextPageFinished;
		public event EventHandler<EventArgs> ReloadFinished;

		private IModelEntityCollection<T> _collection;

		public IModelEntityCollection<T> Collection
		{ 
			get
			{
				return _collection; 
			}
			set
			{
				_collection = value; 
				RaisePropertyChanged(() => Collection); 
				RaisePropertyChanged(() => Count); 
			}
		}

		public int Count
		{ 
			get
			{
				return this._collection != null ? _collection.Count : 0; 
			}
		}

		private bool _isLoadingInitialData;

		public bool IsLoadingInitialData
		{ 
			get
			{
				return _isLoadingInitialData; 
			}
			set
			{
				_isLoadingInitialData = value; 
				RaisePropertyChanged(() => IsLoadingInitialData); 
			}
		}

		private bool _isLoadingNextPage;

		public bool IsLoadingNextPage
		{ 
			get
			{
				return _isLoadingNextPage; 
			}
			set
			{
				_isLoadingNextPage = value; 
				RaisePropertyChanged(() => IsLoadingNextPage); 
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _tryAgainCommand;

		public System.Windows.Input.ICommand TryAgainCommand
		{
			get
			{
				_tryAgainCommand = _tryAgainCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(tryAgain, () => !IsLoading);
				return _tryAgainCommand;
			}
		}


		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFirstPageCommand;

		public System.Windows.Input.ICommand LoadFirstPageCommand
		{
			get
			{
				_loadFirstPageCommand = _loadFirstPageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						_lastExecutedCommand = Commands.LoadNextPage;
						await loadNextPage();
					}, () => CanExecuteLoadNextPageCommand && !_isFirstPageLoaded);
				return _loadFirstPageCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadNextPageCommand;

		public System.Windows.Input.ICommand LoadNextPageCommand
		{
			get
			{
				_loadNextPageCommand = _loadNextPageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						await loadNextPage();
					}, () => CanExecuteLoadNextPageCommand);
				return _loadNextPageCommand;
			}
		}

		public bool CanExecuteLoadNextPageCommand
		{
			get
			{
				return !this.IsLoading;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;
		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
						_lastExecutedCommand = Commands.Reload;
						await reload();
					}, () => CanExecuteReloadCommand);
				return _reloadCommand;
			}
		}

		public bool CanExecuteReloadCommand
		{
			get
			{
				return !this.IsLoading && this.IsNetworkAvailable;
			}
		}

		#endregion

		#region Protected

		protected override void handleNetworkConnectionChanged(object sender, Connectivity.Plugin.Abstractions.ConnectivityChangedEventArgs e)
		{
			base.handleNetworkConnectionChanged(sender, e);
			//Try to refetch data if there is internet connection now
			if (e.IsConnected)
				this.TryAgainCommand.Execute(null);
		}

		#endregion

		#region Utility Methods

		private async Task loadNextPage()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failureMessage = "";
			try
			{
				int initialCount = this.Count;
				if (_collection == null)
				{
					this.IsLoadingInitialData = true;
					var collection = new ModelEntityCollection<T>(_query, _pageSize);
					await collection.FetchNextPageAsync();
					this.Collection = collection;
				}
				else
				{
					this.IsLoadingNextPage = true;
					await _collection.FetchNextPageAsync();
				}
				//If no data was fetched and there was no network connection available warn user
				if(initialCount == this.Count && !this.IsNetworkAvailable)
				{
					failureMessage = AppResources.MessageLoadingFailedNoInternet;
				}
				else
				{
					_isFirstPageLoaded = true;
				}
			}
			catch (Exception ex)
			{
				failureMessage = handleException(ex);
			}
				
			this.IsLoadingNextPage = false;
			this.IsLoadingInitialData = false;
			this.StopLoading(failureMessage);
			LoadingNextPageFinished.Fire(this, new EventArgs());
		}

		private async Task reload()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failureMessage = "";

			try
			{
				await _clearCacheTask();
				var collection = new ModelEntityCollection<T>(_query, _pageSize);
				await collection.FetchNextPageAsync();
				this.Collection = collection;
				//If no data was fetched and there was no network connection available warn user
				if(this.Count == 0 && !this.IsNetworkAvailable)
				{
					failureMessage = AppResources.MessageLoadingFailedNoInternet;
				}
			}
			catch (Exception ex)
			{
				failureMessage = handleException(ex);
			}

			this.StopLoading(failureMessage);
			ReloadFinished.Fire(this, new EventArgs());
		}

		void tryAgain()
		{
			//Try again only if previously something went wrong,
			//that is LoadingFailureMessage is not empty
			if (string.IsNullOrEmpty(this.LoadingFailureMessage))
				return;
			switch (_lastExecutedCommand)
			{
				case Commands.LoadFirstPage:
					LoadFirstPageCommand.Execute(null);
					break;
				case Commands.LoadNextPage:
					LoadNextPageCommand.Execute(null);
					break;
				case Commands.Reload:
					ReloadCommand.Execute(null);
					break;
				default:
					break;
			}
		}

		void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{			
			if (e.PropertyName == IsLoadingPropertyName || e.PropertyName == IsNetworkAvailablePropertyName)
			{
				RaisePropertyChanged(() => CanExecuteLoadNextPageCommand);
			}
		}

		private string handleException(Exception ex)
		{
			string failureMessage = "";
			try
			{
				throw ex;
			}
			catch (OperationCanceledException)
			{
				failureMessage = AppResources.MessageLoadingTimeOut;
			}
			catch (Exception)
			{
				failureMessage = AppResources.MessageLoadingFailed;
			}	
			return failureMessage;
		}

		#endregion
	}
}


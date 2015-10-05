using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Core.Resources;

namespace Tojeero.Core.ViewModels
{
	public class BaseCollectionViewModel<T> : LoadableNetworkViewModel 
		where T : IModelEntity
	{
		#region Private fields and properties
		QueryDelegate<T> _query;
		Func<Task> _clearCacheTask;
		private int _pageSize;
		private bool _isFirstPageLoaded;
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
		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFirstPageCommand;
		public System.Windows.Input.ICommand LoadFirstPageCommand
		{
			get
			{
				_loadFirstPageCommand = _loadFirstPageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
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
				return !this.IsLoading && this.IsNetworkAvailable;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;
		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () =>
					{
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

		#region Utility Methods

		private async Task loadNextPage()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failureMessage = "";
			try
			{
				if(_collection == null)
				{
					this.IsLoadingInitialData = true;
					var collection = new ModelEntityCollection<T>(_query, _pageSize);
					await collection.FetchNextPageAsync();
					this.Collection = collection;
					_isFirstPageLoaded = true;
				}
				else
				{
					this.IsLoadingNextPage = true;
					await _collection.FetchNextPageAsync();
				}
			}
			catch(Exception ex)
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
			}
			catch(Exception ex)
			{
				failureMessage = handleException(ex);
			}
			this.StopLoading(failureMessage);
			ReloadFinished.Fire(this, new EventArgs());
		}

		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{			
			if(e.PropertyName == IsLoadingPropertyName || e.PropertyName == IsNetworkAvailablePropertyName)
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
			catch(OperationCanceledException)
			{
				failureMessage = AppResources.MessageLoadingTimeOut;
			}
			catch(Exception)
			{
				failureMessage = AppResources.MessageLoadingFailed;
			}	
			return failureMessage;
		}
		#endregion
	}
}


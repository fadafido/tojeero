using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Tojeero.Core.ViewModels
{
	public class BaseCollectionViewModel<T> : LoadableNetworkViewModel 
		where T : IModelEntity
	{
		#region Private fields and properties
		QueryDelegate<T> _query;
		private int _pageSize;
		#endregion


		#region Constructors

		public BaseCollectionViewModel(QueryDelegate<T> query, int pageSize = 10)
			: base()
		{			
			_pageSize = pageSize;
			_query = query;
			_collection = new ModelEntityCollection<T>(_query, _pageSize);
			PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

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

		#endregion

		#region Commands

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

		#endregion

		#region Utility Methods

		private async Task loadNextPage()
		{
			this.StartLoading("Loading...");
			string failureMessage = "";
			try
			{
				await _collection.FetchNextPageAsync();
			}
			catch(OperationCanceledException)
			{
				failureMessage = "Loading timed out. Please try again.";
			}
			catch(Exception)
			{
				failureMessage = "Data loading failed. Please try again.";
			}	
			this.StopLoading(failureMessage);
		}

		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{			
			if(e.PropertyName == IsLoadingPropertyName || e.PropertyName == IsNetworkAvailablePropertyName)
			{
				RaisePropertyChanged(() => CanExecuteLoadNextPageCommand);
			}
		}
		#endregion
	}
}


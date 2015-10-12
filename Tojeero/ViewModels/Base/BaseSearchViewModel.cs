using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public abstract class BaseSearchViewModel<T> : LoadableNetworkViewModel 
		where T : IModelEntity
	{
		#region Private fields and properties

		private BaseCollectionViewModel<T> _browsingViewModel;
		private BaseCollectionViewModel<T> _searchViewModel;
		private Timer _timer;
		private readonly object _lockObject = new object();
		private string _previousSearchQuery = null;

		#endregion

		#region Constructors

		public BaseSearchViewModel(string initialSearchQuery = null)
		{
			_searchQuery = initialSearchQuery;
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> LoadingNextPageFinished;
		public event EventHandler<EventArgs> ReloadFinished;

		private BaseCollectionViewModel<T> _viewModel;
		public BaseCollectionViewModel<T> ViewModel
		{ 
			get
			{
				if (_viewModel == null)
				{
					_viewModel = !string.IsNullOrWhiteSpace(_searchQuery) ? GetSearchViewModel(_searchQuery) : GetBrowsingViewModel();
					connectEvents();
				}
				return _viewModel; 
			}
			set
			{
				if (_viewModel != value)
				{
					disconnectEvents();
					_viewModel = value;
					connectEvents();
					RaisePropertyChanged(() => ViewModel); 
				}
			}
		}

		private string _searchQuery;
		public string SearchQuery
		{ 
			get  
			{
				return _searchQuery; 
			}
			set 
			{
				_searchQuery = value; 
				RaisePropertyChanged(() => SearchQuery); 
				ScheduleSearch();
			}
		}

		#endregion

		#region Protected API

		protected abstract BaseCollectionViewModel<T> GetBrowsingViewModel();
		protected abstract BaseCollectionViewModel<T> GetSearchViewModel(string searchQuery);

		#endregion

		#region Utility Methods

		private void ScheduleSearch()
		{
			lock (_lockObject)
			{
				if (_timer != null)
				{
					_timer.Dispose();
					_timer = null;
				}
				_timer = new Timer(state =>
					{
						_timer.Dispose();
						_timer = null;
						doSearch();
					}, null, Constants.SearchTimeout);
			}
		}

		private void doSearch()
		{
			if (string.IsNullOrWhiteSpace(SearchQuery))
			{
				this.ViewModel = getBrowsingViewModel();
				return;
			}

			if (_previousSearchQuery != this.SearchQuery)
			{
				this.ViewModel = GetSearchViewModel(this.SearchQuery);
				this.ViewModel.LoadFirstPageCommand.Execute(null);
			}
		}
			
		BaseCollectionViewModel<T> getBrowsingViewModel()
		{
			return  _browsingViewModel ?? GetBrowsingViewModel();
		}


		private void connectEvents()
		{
			if (this.ViewModel != null)
			{
				this.ViewModel.ReloadFinished += handleReloadFinished;
				this.ViewModel.LoadingNextPageFinished += handleLoadingNextPageFinished;
			}
		}

		private void disconnectEvents()
		{
			if (this.ViewModel != null)
			{
				this.ViewModel.ReloadFinished -= handleReloadFinished;
				this.ViewModel.LoadingNextPageFinished -= handleLoadingNextPageFinished;
			}
		}

		void handleLoadingNextPageFinished (object sender, EventArgs e)
		{
			LoadingNextPageFinished.Fire(sender, e);
		}

		void handleReloadFinished (object sender, EventArgs e)
		{
			ReloadFinished.Fire(sender, e);
		}
		#endregion
	}
}


using System;
using System.ComponentModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public abstract class BaseSearchViewModel<T> : BaseLoadableNetworkViewModel, ISearchViewModel
    {
        #region Private fields and properties

        protected BaseCollectionViewModel<T> _browsingViewModel;
        protected BaseCollectionViewModel<T> _searchViewModel;
        private Timer _timer;
        private readonly object _lockObject = new object();
        private readonly string _previousSearchQuery = null;

        #endregion

        #region Constructors

        public BaseSearchViewModel(string initialSearchQuery = null)
        {
            _searchQuery = initialSearchQuery;
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs> LoadingNextPageFinished;
        public event EventHandler<EventArgs> ReloadFinished;

        private BaseCollectionViewModel<T> _viewModel;
        public static string ViewModelProperty = "ViewModel";

        public BaseCollectionViewModel<T> ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    if (!string.IsNullOrWhiteSpace(_searchQuery))
                    {
                        _viewModel = _searchViewModel = GetSearchViewModel(_searchQuery);
                    }
                    else
                    {
                        _viewModel = _browsingViewModel = GetBrowsingViewModel();
                    }
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
                    _viewModel.LoadFirstPageCommand.Execute(null);
                }
            }
        }

        private string _searchQuery;
        public static string SearchQueryProperty = "SearchQuery";

        public virtual string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                //If the value is empty or null stop searching.
                //Start searching only if the search query contains at least 2 non whitespace characters
                if (_searchQuery != value && (string.IsNullOrEmpty(value) || value.Trim().Length >= MinSearchCharacters))
                {
                    _searchQuery = value;
                    RaisePropertyChanged(() => SearchQuery);
                    ScheduleSearch();
                }
            }
        }

        public virtual int SearchTimeout
        {
            get { return 200; }
        }

        public bool IsSearchbarVisible
        {
            get { return ViewModel.IsInitialDataLoaded || !string.IsNullOrEmpty(SearchQuery); }
        }

        public virtual int MinSearchCharacters
        {
            get { return 2; }
        }

        #endregion

        #region Commands

        private MvxCommand _refetchCommand;

        public ICommand RefetchCommand
        {
            get
            {
                _refetchCommand = _refetchCommand ?? new MvxCommand(() =>
                {
                    if (_browsingViewModel != null)
                        _browsingViewModel.RefetchCommand.Execute(null);
                    if (_searchViewModel != null)
                        _searchViewModel.RefetchCommand.Execute(null);
                });
                return _refetchCommand;
            }
        }

        public ICommand ItemSelectedCommand => null;

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
                    doSearch();
                    _timer.Dispose();
                    _timer = null;
                }, null, SearchTimeout);
            }
        }

        private void doSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                ViewModel = getBrowsingViewModel();
                _searchViewModel = null;
            }
            else if (_previousSearchQuery != SearchQuery)
            {
                ViewModel = _searchViewModel = GetSearchViewModel(SearchQuery);
            }
        }

        BaseCollectionViewModel<T> getBrowsingViewModel()
        {
            if (_browsingViewModel == null)
                _browsingViewModel = GetBrowsingViewModel();
            return _browsingViewModel;
        }

        private void connectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished += HandleReloadFinished;
                ViewModel.LoadingNextPageFinished += HandleLoadingNextPageFinished;
                ViewModel.PropertyChanged += propertyChanged;
            }
        }

        private void disconnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished -= HandleReloadFinished;
                ViewModel.LoadingNextPageFinished -= HandleLoadingNextPageFinished;
                ViewModel.PropertyChanged += propertyChanged;
            }
        }

        protected virtual void HandleLoadingNextPageFinished(object sender, EventArgs e)
        {
            LoadingNextPageFinished.Fire(sender, e);
        }

        protected virtual void HandleReloadFinished(object sender, EventArgs e)
        {
            ReloadFinished.Fire(sender, e);
        }

        protected virtual void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ViewModelProperty || e.PropertyName == SearchQueryProperty ||
                e.PropertyName == BaseCollectionViewModel<T>.IsInitialDataLoadedProperty)
            {
                RaisePropertyChanged(() => IsSearchbarVisible);
            }
        }

        #endregion
    }
}
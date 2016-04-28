using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Connectivity.Plugin.Abstractions;
using Nito.AsyncEx;
using Tojeero.Core.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public class BaseCollectionViewModel<T> : BaseLoadableNetworkViewModel, ICollectionViewModel<T>
    {
        #region Private fields and properties

        private enum Commands
        {
            Unknown,
            LoadFirstPage,
            LoadNextPage,
            Reload,
            Search
        }

        readonly IModelQuery<T> _query;
        private readonly int _pageSize;
        private Commands _lastExecutedCommand;
        private readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public BaseCollectionViewModel(IModelQuery<T> query, int pageSize = 50)
        {
            _pageSize = pageSize;
            _query = query;
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs> LoadingNextPageFinished;
        public event EventHandler<EventArgs> ReloadFinished;

        private IModelEntityCollection<T> _collection;

        public IModelEntityCollection<T> Collection
        {
            get { return _collection; }
            set
            {
                if (_collection != value)
                {
                    IsInitialDataLoaded = false;
                    if (_collection != null)
                    {
                        _collection.CollectionChanged -= collectionChanged;
                    }
                    _collection = value;
                    if (_collection != null)
                    {
                        _collection.CollectionChanged += collectionChanged;
                    }
                    RaisePropertyChanged(() => Collection);
                    RaisePropertyChanged(() => Count);
                }
            }
        }

        public int Count
        {
            get { return _collection != null ? _collection.Count : 0; }
        }

        private bool _isLoadingInitialData;

        public bool IsLoadingInitialData
        {
            get { return _isLoadingInitialData; }
            private set
            {
                _isLoadingInitialData = value;
                RaisePropertyChanged(() => IsLoadingInitialData);
            }
        }

        private bool _isLoadingNextPage;

        public bool IsLoadingNextPage
        {
            get { return _isLoadingNextPage; }
            private set
            {
                _isLoadingNextPage = value;
                RaisePropertyChanged(() => IsLoadingNextPage);
            }
        }

        private bool _isInitialDataLoaded;
        public static string IsInitialDataLoadedProperty = "IsInitialDataLoaded";

        public bool IsInitialDataLoaded
        {
            get { return _isInitialDataLoaded; }
            private set
            {
                _isInitialDataLoaded = value;
                RaisePropertyChanged(() => IsInitialDataLoaded);
                RaisePropertyChanged(() => IsPlaceholderVisible);
            }
        }

        private string _placeholder;

        public string Placeholder
        {
            get { return _placeholder; }
            set
            {
                _placeholder = value;
                RaisePropertyChanged(() => Placeholder);
            }
        }

        public bool IsPlaceholderVisible
        {
            get { return IsInitialDataLoaded && Count == 0; }
        }

        #endregion

        #region Commands

        private MvxCommand _tryAgainCommand;

        public ICommand TryAgainCommand
        {
            get
            {
                _tryAgainCommand = _tryAgainCommand ?? new MvxCommand(tryAgain, () => !IsLoading);
                return _tryAgainCommand;
            }
        }


        private MvxCommand _loadFirstPageCommand;

        public ICommand LoadFirstPageCommand
        {
            get
            {
                _loadFirstPageCommand = _loadFirstPageCommand ??
                                        new MvxCommand(async () => { await loadNextPage(); },
                                            () => CanExecuteLoadNextPageCommand && Count == 0);
                return _loadFirstPageCommand;
            }
        }

        private MvxCommand _refetchCommand;

        public ICommand RefetchCommand
        {
            get
            {
                _refetchCommand = _refetchCommand ?? new MvxCommand(async () =>
                {
                    await Task.Factory.StartNew(() =>
                    {
                        while (!CanExecuteLoadNextPageCommand)
                        {
                        }
                    });
                    Collection = null;
                    await loadNextPage();
                });
                return _refetchCommand;
            }
        }

        private MvxCommand _loadNextPageCommand;

        public ICommand LoadNextPageCommand
        {
            get
            {
                _loadNextPageCommand = _loadNextPageCommand ??
                                       new MvxCommand(async () => { await loadNextPage(); },
                                           () => CanExecuteLoadNextPageCommand);
                return _loadNextPageCommand;
            }
        }

        public bool CanExecuteLoadNextPageCommand
        {
            get { return !IsLoading; }
        }

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ?? new MvxCommand(async () =>
                {
                    if (CanExecuteReloadCommand)
                    {
                        _lastExecutedCommand = Commands.Reload;
                        await reload();
                    }
                    ReloadFinished.Fire(this, new EventArgs());
                });
                return _reloadCommand;
            }
        }

        public bool CanExecuteReloadCommand
        {
            get { return !IsLoading && IsNetworkAvailable; }
        }

        public virtual ICommand ItemSelectedCommand => null;     

        #endregion

        #region Protected

        protected override void handleNetworkConnectionChanged(object sender, ConnectivityChangedEventArgs e)
        {
            base.handleNetworkConnectionChanged(sender, e);
            //Try to refetch data if there is internet connection now
            if (e.IsConnected)
                TryAgainCommand.Execute(null);
        }

        #endregion

        #region Utility Methods

        private async Task loadNextPage()
        {
            using (var writerLock = await _locker.WriterLockAsync())
            {
                _lastExecutedCommand = Commands.LoadNextPage;
                StartLoading(AppResources.MessageGeneralLoading);
                var failureMessage = "";
                try
                {
                    var initialCount = Count;
                    if (_collection == null)
                    {
                        IsLoadingInitialData = true;
                        var collection = new ModelEntityCollection<T>(_query, _pageSize);
                        await collection.FetchNextPageAsync();
                        Collection = collection;
                        IsInitialDataLoaded = true;
                    }
                    else
                    {
                        IsLoadingNextPage = true;
                        await _collection.FetchNextPageAsync();
                    }
                    //If no data was fetched and there was no network connection available warn user
                    if (initialCount == Count && !IsNetworkAvailable)
                    {
                        failureMessage = AppResources.MessageLoadingFailedNoInternet;
                    }
                }
                catch (Exception ex)
                {
                    failureMessage = handleException(ex);
                }

                IsLoadingNextPage = false;
                IsLoadingInitialData = false;
                StopLoading(failureMessage);
                LoadingNextPageFinished.Fire(this, new EventArgs());
            }
        }

        private async Task reload()
        {
            using (var writerLock = await _locker.WriterLockAsync())
            {
                StartLoading(AppResources.MessageGeneralLoading);
                var failureMessage = "";

                try
                {
                    await _query.ClearCache();
                    var collection = new ModelEntityCollection<T>(_query, _pageSize);
                    await collection.FetchNextPageAsync();
                    Collection = collection;
                    IsInitialDataLoaded = true;
                    //If no data was fetched and there was no network connection available warn user
                    if (Count == 0 && !IsNetworkAvailable)
                    {
                        failureMessage = AppResources.MessageLoadingFailedNoInternet;
                    }
                }
                catch (Exception ex)
                {
                    failureMessage = handleException(ex);
                }

                StopLoading(failureMessage);
            }
        }

        void tryAgain()
        {
            //Try again only if previously something went wrong,
            //that is LoadingFailureMessage is not empty
            if (string.IsNullOrEmpty(LoadingFailureMessage))
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

        void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == IsLoadingProperty || e.PropertyName == IsNetworkAvailableProperty)
            {
                RaisePropertyChanged(() => CanExecuteLoadNextPageCommand);
                RaisePropertyChanged(() => CanExecuteReloadCommand);
            }
        }

        void collectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => Count);
            RaisePropertyChanged(() => IsPlaceholderVisible);
        }

        private string handleException(Exception ex)
        {
            var failureMessage = "";
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
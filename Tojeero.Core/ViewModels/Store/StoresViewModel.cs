using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Messages;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Store
{
    public class StoresViewModel : BaseSearchViewModel<StoreViewModel>
    {
        #region Private fields and properties

        private readonly IStoreManager _manager;
        private readonly MvxSubscriptionToken _filterChangeToken;
        private readonly MvxSubscriptionToken _storeChangeToken;
        private readonly MvxSubscriptionToken _sessionChangedToken;

        #endregion

        #region Constructors

        public StoresViewModel(IStoreManager manager, IMvxMessenger messenger)
        {
            _manager = manager;
            _filterChangeToken =
                messenger.SubscribeOnMainThread<StoreFilterChangedMessage>(m => { RefetchCommand.Execute(null); });
            _storeChangeToken =
                messenger.SubscribeOnMainThread<StoreChangedMessage>(message => { RefetchCommand.Execute(null); });
            _sessionChangedToken =
                messenger.SubscribeOnMainThread<SessionStateChangedMessage>(m => { RefetchCommand.Execute(null); });
        }

        #endregion

        #region Lifecycle management

        public override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadFirstPageCommand.Execute(null);
        }

        #endregion

        #region Properties

        public Action<IStore> ShowStoreInfoAction { get; set; }

        #endregion

        #region Commands

        private MvxCommand<StoreViewModel> _itemSelectedCommand;
        public ICommand ItemSelectedCommand
        {
            get
            {
                _itemSelectedCommand = _itemSelectedCommand 
                    ?? new MvxCommand<StoreViewModel>(s => ShowStoreInfoAction(s.Store));
                return _itemSelectedCommand;
            }
        }

        #endregion

        #region Parent override

        protected override BaseCollectionViewModel<StoreViewModel> GetBrowsingViewModel()
        {
            var viewModel = new BaseCollectionViewModel<StoreViewModel>(new StoresQuery(_manager),
                Constants.StoresPageSize);
            viewModel.Placeholder = AppResources.MessageNoStores;
            return viewModel;
        }

        protected override BaseCollectionViewModel<StoreViewModel> GetSearchViewModel(string searchQuery)
        {
            var viewModel = new BaseCollectionViewModel<StoreViewModel>(new SearchStoresQuery(searchQuery, _manager),
                Constants.StoresPageSize);
            viewModel.Placeholder = AppResources.MessageNoStores;
            return viewModel;
        }

        #endregion

        #region Queries

        private class StoresQuery : IModelQuery<StoreViewModel>
        {
            readonly IStoreManager manager;

            public StoresQuery(IStoreManager manager)
            {
                this.manager = manager;
            }

            public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await manager.Fetch(pageSize, offset, RuntimeSettings.StoreFilter);
                return result.Select(p => new StoreViewModel(p));
            }

            public Comparison<StoreViewModel> Comparer
            {
                get { return null; }
            }


            public Task ClearCache()
            {
                return manager.ClearCache();
            }
        }

        private class SearchStoresQuery : IModelQuery<StoreViewModel>
        {
            readonly IStoreManager manager;
            readonly string searchQuery;

            public SearchStoresQuery(string searchQuery, IStoreManager manager)
            {
                this.searchQuery = searchQuery;
                this.manager = manager;
            }

            public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await manager.Find(searchQuery, pageSize, offset, RuntimeSettings.StoreFilter);
                return result.Select(p => new StoreViewModel(p));
            }

            public Comparison<StoreViewModel> Comparer
            {
                get { return null; }
            }

            public Task ClearCache()
            {
                return manager.ClearCache();
            }
        }

        #endregion
    }
}
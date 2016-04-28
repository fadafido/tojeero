using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Product
{
    public class ProductsViewModel : BaseSearchViewModel<ProductViewModel>
    {
        #region Private fields and properties

        private readonly IProductManager _manager;
        private readonly MvxSubscriptionToken _filterChangedToken;
        private readonly MvxSubscriptionToken _sessionChangedToken;
        private readonly MvxSubscriptionToken _productChangeToken;

        #endregion

        #region Constructors

        public ProductsViewModel(IProductManager manager, IMvxMessenger messenger)
        {
            _manager = manager;
            _filterChangedToken =
                messenger.Subscribe<ProductFilterChangedMessage>(m => { RefetchCommand.Execute(null); });
            _productChangeToken =
                messenger.Subscribe<ProductChangedMessage>(message => { RefetchCommand.Execute(null); });
            _sessionChangedToken =
                messenger.Subscribe<SessionStateChangedMessage>(m => { RefetchCommand.Execute(null); });
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

        public Action ShowFiltersAction { get; set; }
        public Action<IProduct> ShowProductDetailsAction { get; set; }

        public Action<ListMode> ChangeListModeAction { get; set; }

        private ListMode _listMode = Settings.ProductListMode;

        public ListMode ListMode
        {
            get { return _listMode; }
            private set
            {
                if (_listMode != value)
                {
                    _listMode = value;
                    updateListMode();
                    RaisePropertyChanged(() => ListMode);
                    RaisePropertyChanged(() => ListModeIcon);
                }
            }
        }

        public string ListModeIcon
        {
            get { return ListMode == ListMode.Normal ? "listLargeCellIcon.png" : "listCellIcon.png"; }
        }

        #endregion

        #region Commands

        private MvxCommand _filterCommand;

        public ICommand FilterCommand
        {
            get
            {
                _filterCommand = _filterCommand ?? new MvxCommand(() => { ShowFiltersAction.Fire(); });
                return _filterCommand;
            }
        }

        private MvxCommand _toggleListModeCommand;

        public ICommand ToggleListModeCommand
        {
            get
            {
                _toggleListModeCommand = _toggleListModeCommand ??
                                         new MvxCommand(
                                             () =>
                                             {
                                                 ListMode = ListMode == ListMode.Normal
                                                     ? ListMode.Large
                                                     : ListMode.Normal;
                                             });
                return _toggleListModeCommand;
            }
        }

        private MvxCommand<ProductViewModel> _showProductDetailsCommand;
        public ICommand ShowProductDetailsCommand
        {
            get
            {
                _showProductDetailsCommand = _showProductDetailsCommand ?? 
                    new MvxCommand<ProductViewModel>(p => ShowProductDetailsAction(p.Product));
                return _showProductDetailsCommand;
            }
        }

        #endregion

        #region Parent override

        protected override BaseCollectionViewModel<ProductViewModel> GetBrowsingViewModel()
        {
            var viewModel = new BaseCollectionViewModel<ProductViewModel>(new ProductsQuery(_manager),
                Constants.ProductsPageSize);
            viewModel.Placeholder = AppResources.MessageNoProducts;
            return viewModel;
        }

        protected override BaseCollectionViewModel<ProductViewModel> GetSearchViewModel(string searchQuery)
        {
            var viewModel = new BaseCollectionViewModel<ProductViewModel>(
                new SearchProductsQuery(searchQuery, _manager), Constants.ProductsPageSize);
            viewModel.Placeholder = AppResources.MessageNoProducts;
            return viewModel;
        }

        #endregion

        #region Queries

        private class ProductsQuery : IModelQuery<ProductViewModel>
        {
            readonly IProductManager manager;

            public ProductsQuery(IProductManager manager)
            {
                this.manager = manager;
            }

            public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await manager.Fetch(pageSize, offset, RuntimeSettings.ProductFilter);
                return result.Select(p => new ProductViewModel(p));
            }

            public Comparison<ProductViewModel> Comparer
            {
                get { return null; }
            }


            public Task ClearCache()
            {
                return manager.ClearCache();
            }
        }

        private class SearchProductsQuery : IModelQuery<ProductViewModel>
        {
            readonly IProductManager manager;
            readonly string searchQuery;

            public SearchProductsQuery(string searchQuery, IProductManager manager)
            {
                this.searchQuery = searchQuery;
                this.manager = manager;
            }

            public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await manager.Find(searchQuery, pageSize, offset, RuntimeSettings.ProductFilter);
                return result.Select(p => new ProductViewModel(p));
            }

            public Comparison<ProductViewModel> Comparer
            {
                get { return null; }
            }

            public Task ClearCache()
            {
                return manager.ClearCache();
            }
        }

        #endregion

        #region Utility methods

        private void updateListMode()
        {
            Settings.ProductListMode = ListMode;
            ChangeListModeAction.Fire(ListMode);
        }

        #endregion
    }
}
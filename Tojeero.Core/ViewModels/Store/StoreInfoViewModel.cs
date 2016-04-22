using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Product;

namespace Tojeero.Core.ViewModels.Store
{
    public class StoreInfoViewModel : StoreViewModel
    {
        #region Private APIs and Fields

        private readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
        private readonly MvxSubscriptionToken _productChangeToken;
        private StoreProductsQuery _query;

        #endregion

        #region Constructors

        public StoreInfoViewModel(IStore store = null, ContentMode mode = ContentMode.View)
            : base(store)
        {
            ShouldSubscribeToSessionChange = true;
            Store = store;
            Mode = mode;
            PropertyChanged += propertyChanged;
            var messenger = Mvx.Resolve<IMvxMessenger>();
            _productChangeToken = messenger.Subscribe<ProductChangedMessage>(message =>
            {
                if (Products != null && message.Product != null &&
                    Store != null && message.Product.StoreID == Store.ID)
                {
                    Products.RefetchCommand.Execute(null);
                }
            });
        }

        #endregion

        #region Properties

        public Action<IStore> ShowStoreDetailsAction { get; set; }
        public Action<IProduct, IStore> AddProductAction { get; set; }

        private BaseCollectionViewModel<ProductViewModel> _products;

        public BaseCollectionViewModel<ProductViewModel> Products
        {
            get { return _products; }
            private set
            {
                _products = value;
                RaisePropertyChanged(() => Products);
            }
        }

        public override IStore Store
        {
            get { return base.Store; }
            set
            {
                base.Store = value;
                setupViewModel();
                RaisePropertyChanged(() => Store);
            }
        }

        private ContentMode _mode;

        public ContentMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                if (_query != null)
                    _query.includeInvisible = _mode == ContentMode.Edit;
                RaisePropertyChanged(() => Mode);
            }
        }

        public bool IsAddFirstProductPlaceholderVisible
        {
            get { return Mode == ContentMode.Edit && IsPlaceholderVisible; }
        }

        public bool IsNoProductsPlaceholderVisible
        {
            get { return Mode == ContentMode.View && IsPlaceholderVisible; }
        }

        public bool IsPlaceholderVisible
        {
            get { return Products != null && Products.IsInitialDataLoaded && Products.Count == 0; }
        }

        public bool IsInEditMode
        {
            get { return Mode == ContentMode.Edit; }
        }

        #endregion

        #region Commands

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ??
                                 new MvxCommand(async () => { await reload(); }, () => !IsLoading && IsNetworkAvailable);
                return _reloadCommand;
            }
        }

        private MvxCommand _showStoreDetailsCommand;

        public ICommand ShowStoreDetailsCommand
        {
            get
            {
                _showStoreDetailsCommand = _showStoreDetailsCommand ??
                                           new MvxCommand(() => { ShowStoreDetailsAction.Fire(Store); });
                return _showStoreDetailsCommand;
            }
        }

        private MvxCommand _addProductCommand;

        public ICommand AddProductCommand
        {
            get
            {
                _addProductCommand = _addProductCommand ?? new MvxCommand(() =>
                {
                    if (AddProductAction != null)
                        AddProductAction(null, Store);
                }, () => true);
                return _addProductCommand;
            }
        }

        #endregion

        #region Queries

        private class StoreProductsQuery : IModelQuery<ProductViewModel>
        {
            readonly IStore store;
            readonly IProductManager productManager;
            public bool includeInvisible;

            public StoreProductsQuery(IProductManager productManager, IStore store, bool includeInvisible = false)
            {
                this.includeInvisible = includeInvisible;
                this.productManager = productManager;
                this.store = store;
            }

            public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await store.FetchProducts(pageSize, offset, includeInvisible);
                return result.Select(p => new ProductViewModel(p));
            }

            public Comparison<ProductViewModel> Comparer
            {
                get { return Comparers.ProductName; }
            }

            public Task ClearCache()
            {
                return productManager.ClearCache();
            }
        }

        #endregion

        #region Utility methods

        protected virtual async Task reload()
        {
            using (var writerLock = await _locker.WriterLockAsync())
            {
                if (Store != null)
                    await Store.LoadRelationships();
                await loadFavorite();
            }
        }

        private void setupViewModel()
        {
            var productManager = Mvx.Resolve<IProductManager>();
            if (Store != null)
            {
                _query = new StoreProductsQuery(productManager, Store, Mode == ContentMode.Edit);
                Products = new BaseCollectionViewModel<ProductViewModel>(_query, Constants.ProductsPageSize);
                Products.PropertyChanged += propertyChanged;
            }
            else
            {
                Products = null;
            }
        }

        void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Mode" || e.PropertyName == "Products" || e.PropertyName == "Count" ||
                e.PropertyName == "IsInitialDataLoaded")
            {
                RaisePropertyChanged(() => IsAddFirstProductPlaceholderVisible);
                RaisePropertyChanged(() => IsNoProductsPlaceholderVisible);
                RaisePropertyChanged(() => IsPlaceholderVisible);
                RaisePropertyChanged(() => IsInEditMode);
            }
        }

        #endregion
    }
}
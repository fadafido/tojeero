using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core.ViewModels
{
	public class StoreInfoViewModel : StoreViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
		private readonly MvxSubscriptionToken _productChangeToken;
		private StoreProductsQuery _query;

		#endregion

		#region Constructors

		public StoreInfoViewModel(IStore store = null, ContentMode mode = ContentMode.View)
			: base(store)
		{
			this.ShouldSubscribeToSessionChange = true;
			this.Store = store;
			this.Mode = mode;
			this.PropertyChanged += propertyChanged;
			var messenger = Mvx.Resolve<IMvxMessenger>();
			_productChangeToken = messenger.Subscribe<ProductChangedMessage>((message) =>
				{
					if(this.Products != null && message.Product != null && 
						this.Store != null && message.Product.StoreID == this.Store.ID)
					{
						this.Products.RefetchCommand.Execute(null);
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
			get
			{
				return _products; 
			}
			private set
			{
				_products = value; 
				RaisePropertyChanged(() => Products); 
			}
		}

		public IStore Store
		{
			get
			{
				return base.Store;
			}
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
			get
			{
				return _mode; 
			}
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
			get
			{
				return this.Mode == ContentMode.Edit && this.IsPlaceholderVisible;
			}
		}

		public bool IsNoProductsPlaceholderVisible
		{
			get
			{ 
				return this.Mode == ContentMode.View && this.IsPlaceholderVisible;
			}
		}

		public bool IsPlaceholderVisible
		{
			get
			{ 
				return this.Products != null && this.Products.IsInitialDataLoaded && this.Products.Count == 0;
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await reload();
				}, () => !IsLoading && IsNetworkAvailable);
				return _reloadCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showStoreDetailsCommand;

		public System.Windows.Input.ICommand ShowStoreDetailsCommand
		{
			get
			{
				_showStoreDetailsCommand = _showStoreDetailsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowStoreDetailsAction.Fire(this.Store);
				});
				return _showStoreDetailsCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _addProductCommand;

		public System.Windows.Input.ICommand AddProductCommand
		{
			get
			{
				_addProductCommand = _addProductCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() =>
					{
						if(AddProductAction != null)
							AddProductAction(null, this.Store);
					}, () => true);
				return _addProductCommand;
			}
		}

		#endregion

		#region Queries

		private class StoreProductsQuery : IModelQuery<ProductViewModel>
		{
			IStore store;
			IProductManager productManager;
			public bool includeInvisible;

			public StoreProductsQuery (IProductManager productManager, IStore store, bool includeInvisible = false)
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
				get
				{
					return Comparers.ProductName;
				}
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
				await loadFavorite();
			}
		}

		private void setupViewModel()
		{
			var productManager = Mvx.Resolve<IProductManager>();
			if (this.Store != null)
			{
				this._query = new StoreProductsQuery(productManager, this.Store, this.Mode == ContentMode.Edit);
				this.Products = new BaseCollectionViewModel<ProductViewModel>(_query, Constants.ProductsPageSize);
				this.Products.PropertyChanged += propertyChanged;
			}
			else
			{
				this.Products = null;
			}
		}

		void propertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Mode" || e.PropertyName == "Products" || e.PropertyName == "Count" || e.PropertyName == "IsInitialDataLoaded")
			{
				RaisePropertyChanged(() => IsAddFirstProductPlaceholderVisible);
				RaisePropertyChanged(() => IsNoProductsPlaceholderVisible);
				RaisePropertyChanged(() => IsPlaceholderVisible);
			}
		}

		#endregion
	}
}


using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class StoreInfoViewModel : StoreViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public StoreInfoViewModel(IStore store = null)
			: base(store)
		{
			this.Store = store;
		}

		#endregion


		#region Properties

		public Action<IStore> ShowStoreDetailsAction;

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

		#endregion

		#region Queries

		private class StoreProductsQuery : IModelQuery<ProductViewModel>
		{
			IStore store;
			IProductManager productManager;

			public StoreProductsQuery (IProductManager productManager, IStore store)
			{
				this.productManager = productManager;
				this.store = store;

			}

			public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await store.FetchProducts(pageSize, offset);
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
				this.Products = new BaseCollectionViewModel<ProductViewModel>(new StoreProductsQuery(productManager, this.Store), Constants.ProductsPageSize);
			}
			else
			{
				this.Products = null;
			}
		}

		#endregion
	}
}


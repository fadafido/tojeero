﻿using System;
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

		public StoreInfoViewModel(IStore store = null, ContentMode mode = ContentMode.View)
			: base(store)
		{
			this.ShouldSubscribeToSessionChange = true;
			this.Store = store;
			this.Mode = mode;
			this.PropertyChanged += propertyChanged;
		}

		#endregion


		#region Properties

		public Action<IStore> ShowStoreDetailsAction { get; set; }
		public Action AddProductAction { get; set; }

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
				RaisePropertyChanged(() => Mode); 
			}
		}

		public bool IsAddFirstProductPlaceholderVisible
		{
			get
			{
				return true;
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
				return true;
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
						AddProductAction.Fire();
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


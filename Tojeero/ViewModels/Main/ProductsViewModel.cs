﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseSearchViewModel<ProductViewModel>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;
		private readonly MvxSubscriptionToken _token;

		private static Comparison<ProductViewModel> _comparer;
		public static Comparison<ProductViewModel> Comparer
		{
			get
			{
				if (_comparer == null)
				{
					_comparer = new Comparison<ProductViewModel>((x, y) =>
						{
							if(x.Product == null || y.Product == null)
								return -1;
							if(x.Product.ID == y.Product.ID)
								return 0;
							if(x.Product.LowercaseName == null || y.Product.LowercaseName == null)
								return -1;
							return x.Product.LowercaseName.CompareIgnoreCase(y.Product.LowercaseName);
						});
				}
				return _comparer;
			}
		}
		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager, IMvxMessenger messenger)
			: base()
		{
			_manager = manager;
			_token = messenger.Subscribe<ProductFilterChangedMessage>((m) =>
				{
					this.ViewModel.RefetchCommand.Execute(null);
				});
		}

		#endregion

		#region implemented abstract members of BaseSearchViewModel

		protected override BaseCollectionViewModel<ProductViewModel> GetBrowsingViewModel()
		{
			return new BaseCollectionViewModel<ProductViewModel>(new ProductsQuery(_manager), Constants.ProductsPageSize);
		}

		protected override BaseCollectionViewModel<ProductViewModel> GetSearchViewModel(string searchQuery)
		{
			return new BaseCollectionViewModel<ProductViewModel>(new SearchProductsQuery(searchQuery, _manager), Constants.ProductsPageSize);
		}

		#endregion

		#region Queries

		private class ProductsQuery : IModelQuery<ProductViewModel>
		{
			IProductManager manager;
			public ProductsQuery (IProductManager manager)
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
				get
				{
					return ProductsViewModel.Comparer;
				}
			}

			
			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchProductsQuery : IModelQuery<ProductViewModel>
		{
			IProductManager manager;
			string searchQuery;

			public SearchProductsQuery (string searchQuery, IProductManager manager)
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
				get
				{
					return ProductsViewModel.Comparer;
				}
			}

			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}
		#endregion
	}
}


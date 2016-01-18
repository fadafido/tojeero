using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;
using System.Linq;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core
{
	public class ProductManager : IProductManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;
		private readonly IMvxMessenger _messenger;
		private readonly ICountryManager _countryManager;

		#endregion

		#region Constructors

		public ProductManager(IModelEntityManager manager, IMvxMessenger messenger, ICountryManager countryManager)
			: base()
		{
			this._manager = manager;
			this._messenger = messenger;
			this._countryManager = countryManager;
		}

		#endregion

		#region IProductManager implementation

		public async Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset, IProductFilter filter = null)
		{
			var products = await _manager.Fetch<IProduct, Product>(new FetchProductsQuery(pageSize, offset, _manager, filter), Constants.ProductsCacheTimespan.TotalMilliseconds);
			await setCountries(products);
			return products;
		}

		public Task<IEnumerable<IProduct>> FetchFavorite(int pageSize, int offset)
		{
			return _manager.Fetch<IProduct, Product>(new FetchFavoriteProductsQuery(pageSize, offset, _manager), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task<int> CountFavorite()
		{
			return _manager.Rest.CountFavoriteProducts();
		}

		public async Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset, IProductFilter filter = null)
		{
			
			var products = await _manager.Fetch<IProduct, Product>(new FindProductsQuery(query, pageSize, offset, _manager, filter), Constants.ProductsCacheTimespan.TotalMilliseconds);
			await setCountries(products);
			return products;
		}

		public Task ClearCache()
		{
			return _manager.Cache.Clear<Product>();
		}


		public async Task<IProduct> Save(ISaveProductViewModel product)
		{
			if (product != null)
			{
				if (product.HasChanged)
				{
					var result = await _manager.Rest.SaveProduct(product);
					if (result != null)
					{
						_messenger.Publish<ProductChangedMessage>(new ProductChangedMessage(this, result, product.IsNew ? EntityChangeType.Create : EntityChangeType.Update));
					}
					return result;
				}
				else
				{
					return product.CurrentProduct;
				}
			}
			return null;
		}

		#endregion

		#region Utility methods

		private async Task setCountries(IEnumerable<IProduct> products)
		{
			if (_countryManager.Countries == null)
			{
				await _countryManager.LoadCountries();
			}
			if (_countryManager.Countries != null)
			{
				foreach (var product in products)
				{
					if(product.CountryId != null)
						product.Country = _countryManager.Countries[product.CountryId];
				}
			}
		}

		#endregion
	}

	#region Queries

	public class FetchProductsQuery : IQueryLoader<IProduct>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		IProductFilter filter;

		public FetchProductsQuery(int pageSize, int offset, IModelEntityManager manager, IProductFilter filter = null)
		{
			this.filter = filter;
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				//TODO:Currently we disable caching. In future phases we'll work on caching.
				return null;
				return this.ToString();
			}
		}

		public async Task<IEnumerable<IProduct>> LocalQuery()
		{
			return await manager.Cache.FetchProducts(pageSize, offset, filter);
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			return await manager.Rest.FetchProducts(pageSize, offset, filter);
		}

		public async Task PostProcess(IEnumerable<IProduct> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Product>());
			foreach (var p in items)
			{
				await manager.Cache.SaveProductTags(p.ID, p.Tags);
			}
		}

		public override string ToString()
		{
			string cachedQueryId = string.Format("products:p_{0}o_{1}-f_{2}", pageSize, offset, filter);
			return cachedQueryId;
		}
	}

	public class FetchFavoriteProductsQuery : IQueryLoader<IProduct>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;

		public FetchFavoriteProductsQuery(int pageSize, int offset, IModelEntityManager manager)
		{
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				//TODO:Currently we disable caching. In future phases we'll work on caching.
				return null;
				return this.ToString();
			}
		}

		public async Task<IEnumerable<IProduct>> LocalQuery()
		{
			return await manager.Cache.FetchFavoriteProducts(pageSize, offset);
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			return await manager.Rest.FetchFavoriteProducts(pageSize, offset);
		}

		public async Task PostProcess(IEnumerable<IProduct> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Product>());
			foreach (var p in items)
			{
				await manager.Cache.SaveProductTags(p.ID, p.Tags);
			}
		}

		public override string ToString()
		{
			string cachedQueryId = string.Format("favorite_products:p_{0}o_{1}", pageSize, offset);
			return cachedQueryId;
		}
	}

	public class FindProductsQuery : IQueryLoader<IProduct>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		string searchQuery;
		IProductFilter filter;

		public FindProductsQuery(string searchQuery, int pageSize, int offset, IModelEntityManager manager, IProductFilter filter = null)
		{
			this.filter = filter;
			this.searchQuery = searchQuery;
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				//TODO:Currently we disable caching. In future phases we'll work on caching.
				return null;
				return this.ToString();
			}
		}

		public async Task<IEnumerable<IProduct>> LocalQuery()
		{
			var result = await manager.Cache.FindProducts(searchQuery, pageSize, offset, filter);
			return result;
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			var result = await manager.Rest.FindProducts(searchQuery, pageSize, offset, filter);
			return result;
		}

		public async Task PostProcess(IEnumerable<IProduct> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Product>());
			foreach (var p in items)
			{
				await manager.Cache.SaveProductTags(p.ID, p.Tags);
			}
		}

		public override string ToString()
		{
			var searchTokens = searchQuery.Tokenize().SubCollection(0, Constants.ParseContainsAllLimit);
			string cachedQueryId = string.Format("products:p_{0}o_{1}-s_{2}-f_{3}", pageSize, offset, string.Join(",", searchTokens), filter);
			return cachedQueryId;
		}
	}

	#endregion
}


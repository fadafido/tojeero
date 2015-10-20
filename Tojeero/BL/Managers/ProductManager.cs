using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;
using System.Linq;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
	public class ProductManager : IProductManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public ProductManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IProductManager implementation

		public Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset, IProductFilter filter = null)
		{
			return _manager.Fetch<IProduct, Product>(new FetchProductsQuery(pageSize, offset, _manager, filter), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset, IProductFilter filter = null)
		{
			return _manager.Fetch<IProduct, Product>(new FindProductsQuery(query, pageSize, offset, _manager, filter), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task ClearCache()
		{
			return _manager.Cache.Clear<Product>();
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


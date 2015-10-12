﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;
using System.Linq;

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

		public Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset)
		{
			return _manager.Fetch<IProduct, Product>(new FetchProductsQuery(pageSize, offset, _manager), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset)
		{
			return _manager.Fetch<IProduct, ParseProduct>(new FindProductsQuery(query, pageSize, offset, _manager), Constants.ProductsCacheTimespan.TotalMilliseconds);
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

		public FetchProductsQuery(int pageSize, int offset, IModelEntityManager manager)
		{
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				string cachedQueryId = string.Format("products-p{0}o{1}", pageSize, offset);
				return cachedQueryId;
			}
		}

		public async Task<IEnumerable<IProduct>> LocalQuery()
		{
			return await manager.Cache.FetchProducts(pageSize, offset);
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			return await manager.Rest.FetchProducts(pageSize, offset);
		}
	}

	public class FindProductsQuery : IQueryLoader<IProduct>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		string query;

		public FindProductsQuery(string query, int pageSize, int offset, IModelEntityManager manager)
		{
			this.query = query;
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				string cachedQueryId = string.Format("products-p{0}o{1}-{2}", pageSize, offset, query);
				return null;
			}
		}

		public async Task<IEnumerable<IProduct>> LocalQuery()
		{
			return await manager.Cache.FindProducts(query, pageSize, offset);
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			return await manager.Rest.FindProducts(query, pageSize, offset);
		}
	}

	#endregion
}


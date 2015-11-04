using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{

	public class StoreProductsQueryLoader : IQueryLoader<IProduct>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		IStore store;

		public StoreProductsQueryLoader(int pageSize, int offset, IModelEntityManager manager, IStore store)
		{
			this.store = store;
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
			return await manager.Cache.FetchStoreProducts(store.ID, pageSize, offset);
		}

		public async Task<IEnumerable<IProduct>> RemoteQuery()
		{
			return await manager.Rest.FetchStoreProducts(store.ID, pageSize, offset);
		}

		public async Task PostProcess(IEnumerable<IProduct> items)
		{
			
		}

		public override string ToString()
		{
			string cachedQueryId = string.Format("store_products:p_{0}o_{1}-s_{2}", pageSize, offset, store.ID);
			return cachedQueryId;
		}
	}
	
}

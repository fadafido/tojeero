using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
	public class StoreManager : IStoreManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public StoreManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IStoreManager Implementation

		public Task<IEnumerable<IStore>> Fetch(int pageSize, int offset)
		{
			return _manager.Fetch<IStore, Store>(new FetchStoresQuery(pageSize, offset, _manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IStore>> Find(string query, int pageSize, int offset)
		{
			return _manager.Fetch<IStore, Store>(new FindStoresQuery(query, pageSize, offset, _manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task ClearCache()
		{
			return _manager.Cache.Clear<Store>();
		}

		#endregion
	}

	#region Queries

	public class FetchStoresQuery : IQueryLoader<IStore>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;

		public FetchStoresQuery(int pageSize, int offset, IModelEntityManager manager)
		{
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				string cachedQueryId = string.Format("stores-p{0}o{1}", pageSize, offset);
				return cachedQueryId;
			}
		}

		public async Task<IEnumerable<IStore>> LocalQuery()
		{
			return await manager.Cache.FetchStores(pageSize, offset);
		}

		public async Task<IEnumerable<IStore>> RemoteQuery()
		{
			return await manager.Rest.FetchStores(pageSize, offset);
		}

		public async Task PostProcess(IEnumerable<IStore> items)
		{
		}
	}

	public class FindStoresQuery : IQueryLoader<IStore>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		string searchQuery;

		public FindStoresQuery(string searchQuery, int pageSize, int offset, IModelEntityManager manager)
		{
			this.searchQuery = searchQuery;
			this.manager = manager;
			this.offset = offset;
			this.pageSize = pageSize;

		}

		public string ID
		{
			get
			{
				string cachedQueryId = string.Format("stores-p{0}o{1}-{2}", pageSize, offset, string.Join(",",searchQuery.Tokenize()));
				return cachedQueryId;
			}
		}

		public async Task<IEnumerable<IStore>> LocalQuery()
		{
			return await manager.Cache.FindStores(searchQuery, pageSize, offset);
		}

		public async Task<IEnumerable<IStore>> RemoteQuery()
		{
			return await manager.Rest.FindStores(searchQuery, pageSize, offset);
		}

		public async Task PostProcess(IEnumerable<IStore> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Store>());
		}
	}
	#endregion
}


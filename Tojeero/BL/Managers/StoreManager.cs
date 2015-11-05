using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;

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

		public Task<IEnumerable<IStore>> Fetch(int pageSize, int offset, IStoreFilter filter = null)
		{
			return _manager.Fetch<IStore, Store>(new FetchStoresQuery(pageSize, offset, _manager, filter), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IStore>> FetchFavorite(int pageSize, int offset)
		{
			return _manager.Fetch<IStore, Store>(new FetchFavoriteStoresQuery(pageSize, offset, _manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IStore>> Find(string query, int pageSize, int offset, IStoreFilter filter = null)
		{
			return _manager.Fetch<IStore, Store>(new FindStoresQuery(query, pageSize, offset, _manager, filter), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public async Task Save(IStoreViewModel store)
		{
			await _manager.Rest.SaveStore(store);
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
		IStoreFilter filter;

		public FetchStoresQuery(int pageSize, int offset, IModelEntityManager manager, IStoreFilter filter = null)
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

		public async Task<IEnumerable<IStore>> LocalQuery()
		{
			return await manager.Cache.FetchStores(pageSize, offset, filter);
		}

		public async Task<IEnumerable<IStore>> RemoteQuery()
		{
			return await manager.Rest.FetchStores(pageSize, offset, filter);
		}

		public async Task PostProcess(IEnumerable<IStore> items)
		{
		}

		public override string ToString()
		{
			string cachedQueryId = string.Format("stores:p_{0}o_{1}-f_{2}", pageSize, offset, filter);
			return cachedQueryId;
		}
	}

	public class FetchFavoriteStoresQuery : IQueryLoader<IStore>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;

		public FetchFavoriteStoresQuery(int pageSize, int offset, IModelEntityManager manager)
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

		public async Task<IEnumerable<IStore>> LocalQuery()
		{
			return await manager.Cache.FetchFavoriteStores(pageSize, offset);
		}

		public async Task<IEnumerable<IStore>> RemoteQuery()
		{
			return await manager.Rest.FetchFavoriteStores(pageSize, offset);
		}

		public async Task PostProcess(IEnumerable<IStore> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Store>());
		}

		public override string ToString()
		{
			string cachedQueryId = string.Format("favorite_stores:p_{0}o_{1}", pageSize, offset);
			return cachedQueryId;
		}
	}

	public class FindStoresQuery : IQueryLoader<IStore>
	{
		int pageSize;
		int offset;
		IModelEntityManager manager;
		string searchQuery;
		IStoreFilter filter;

		public FindStoresQuery(string searchQuery, int pageSize, int offset, IModelEntityManager manager, IStoreFilter fil\ter = null)
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

		public async Task<IEnumerable<IStore>> LocalQuery()
		{
			return await manager.Cache.FindStores(searchQuery, pageSize, offset, filter);
		}

		public async Task<IEnumerable<IStore>> RemoteQuery()
		{
			return await manager.Rest.FindStores(searchQuery, pageSize, offset, filter);
		}

		public async Task PostProcess(IEnumerable<IStore> items)
		{
			await manager.Cache.SaveSearchTokens(items, CachedQuery.GetEntityCacheName<Store>());
		}

		public override string ToString()
		{
			var searchTokens = searchQuery.Tokenize().SubCollection(0, Constants.ParseContainsAllLimit);
			string cachedQueryId = string.Format("stores:p_{0}o_{1}-s_{2}-f_{3}", pageSize, offset, string.Join(",", searchTokens), filter);
			return cachedQueryId;
		}
	}
	#endregion
}


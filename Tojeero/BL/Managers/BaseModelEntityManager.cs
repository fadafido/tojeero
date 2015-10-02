using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public class BaseModelEntityManager : IModelEntityManager
	{
		#region Private Fields and Properties

		private readonly IRestRepository _restRepository;
		private readonly ICacheRepository _cacheRepository;

		#endregion

		#region Constructors

		public BaseModelEntityManager(ICacheRepository cacheRepository, IRestRepository restRepository)
		{
			this._restRepository = restRepository;
			this._cacheRepository = cacheRepository;
		}

		#endregion

		#region IModelEntityManager implementation

		public IRestRepository Rest
		{
			get
			{
				return _restRepository;
			}
		}

		public ICacheRepository Cache
		{
			get
			{
				return _cacheRepository;
			}
		}

		#endregion

		#region IRepository implementation

		public Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			string cachedQueryId = string.Format("products-p{0}o{1}", pageSize, offset);
			var local = new Task<IEnumerable<IProduct>>(() => Cache.FetchProducts(pageSize, offset).Result);
			var remote = new Task<IEnumerable<IProduct>>(() => Rest.FetchProducts(pageSize, offset).Result);
			return fetch<IProduct>(cachedQueryId, local, remote);
		}

		public Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			string cachedQueryId = string.Format("stores-p{0}o{1}", pageSize, offset);
			var local = new Task<IEnumerable<IStore>>(() => Cache.FetchStores(pageSize, offset).Result);
			var remote = new Task<IEnumerable<IStore>>(() => Rest.FetchStores(pageSize, offset).Result);
			return fetch<IStore>(cachedQueryId, local, remote);
		}

		#endregion

		#region Utility methods

		public async Task<IEnumerable<T>> fetch<T>(string cachedQueryId, Task<IEnumerable<T>> localQuery, Task<IEnumerable<T>> remoteQuery, double? expiresIn = null)
		{
			var cachedQuery = await Cache.FetchObjectAsync<CachedQuery>(cachedQueryId);
			IEnumerable<T> result = null;
			//If the query has not ever been executed or was expired fetch the results from backend and save them to local cache
			if (cachedQuery == null || cachedQuery.IsExpired)
			{
				result = await remoteQuery;
				await Cache.SaveAsync(result);
				cachedQuery = new CachedQuery()
				{
					ID = cachedQueryId,
					LastFetchedAt = DateTime.UtcNow,
					ExpiresIn = expiresIn
				};
				await Cache.SaveAsync(cachedQuery);
			}
			else
			{
				result = await localQuery;
			}
			return result;
		}

		#endregion
	}
}


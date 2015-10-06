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
			return fetch<IProduct, Product>(new FetchProductsQuery(pageSize, offset, this), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			string cachedQueryId = string.Format("stores-p{0}o{1}", pageSize, offset);
			return fetch<IStore, Store>(new FetchStoresQuery(pageSize, offset, this), Constants.StoresCacheTimespan.TotalMilliseconds);
		}


		public Task<IEnumerable<ICountry>> FetchCountries()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Utility methods

		private async Task<IEnumerable<T>> fetch<T, Entity>(IQueryLoader<T> loader, double? expiresIn = null)
		{
			IEnumerable<T> result = null;
			var cacheName = CachedQuery.GetEntityCacheName<Entity>();
			var cachedQuery = await Cache.FetchObjectAsync<CachedQuery>(loader.ID).ConfigureAwait(false);
			bool isExpired = cachedQuery == null || cachedQuery.IsExpired;

			//If the query has not ever been executed or was expired fetch the results from backend and save them to local cache
			if (isExpired)
			{
				result = await loader.RemoteQuery().ConfigureAwait(false);
				await Cache.SaveAsync(result).ConfigureAwait(false);
				cachedQuery = new CachedQuery()
				{
					ID = loader.ID,
					EntityName = cacheName,
					LastFetchedAt = DateTime.UtcNow,
					ExpiresIn = expiresIn
				};
				await Cache.SaveAsync(cachedQuery).ConfigureAwait(false);
			}
			else
			{
				result = await loader.LocalQuery().ConfigureAwait(false);
			}
			return result;
		}

		private interface IQueryLoader<T>
		{
			string ID { get; }

			Task<IEnumerable<T>> LocalQuery();

			Task<IEnumerable<T>> RemoteQuery();
		}

		private class FetchProductsQuery : IQueryLoader<IProduct>
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

			#region IQueryLoader implementation

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

			#endregion
		}

		private class FetchStoresQuery : IQueryLoader<IStore>
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

			#region IQueryLoader implementation

			public string ID
			{
				get
				{
					string cachedQueryId = string.Format("products-p{0}o{1}", pageSize, offset);
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

			#endregion
		}

		#endregion
	}
}


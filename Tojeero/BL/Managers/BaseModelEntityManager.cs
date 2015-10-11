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
			return fetch<IStore, Store>(new FetchStoresQuery(pageSize, offset, this), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<ICountry>> FetchCountries()
		{
			return fetch<ICountry, Country>(new FetchCountriesQuery(this), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<ICity>> FetchCities(int countryId)
		{
			return fetch<ICity, City>(new FetchCitiesQuery(countryId, this), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset)
		{
			return fetch<IProduct, ParseProduct>(new FindProductsQuery(query, pageSize, offset, this), Constants.ProductsCacheTimespan.TotalMilliseconds);
		}

		public Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset)
		{
			return fetch<IStore, ParseStore>(new FindStoresQuery(query, pageSize, offset, this), Constants.StoresCacheTimespan.TotalMilliseconds);
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
				if (!string.IsNullOrEmpty(loader.ID))
				{
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
		}


		private class FetchCountriesQuery : IQueryLoader<ICountry>
		{
			IModelEntityManager manager;

			public FetchCountriesQuery(IModelEntityManager manager)
			{
				this.manager = manager;
			}

			public string ID
			{
				get
				{
					return "countries";
				}
			}

			public async Task<IEnumerable<ICountry>> LocalQuery()
			{
				return await manager.Cache.FetchCountries();
			}

			public async Task<IEnumerable<ICountry>> RemoteQuery()
			{
				return await manager.Rest.FetchCountries();
			}
		}

		private class FetchCitiesQuery : IQueryLoader<ICity>
		{
			IModelEntityManager manager;
			int countryId;

			public FetchCitiesQuery(int countryId, IModelEntityManager manager)
			{
				this.countryId = countryId;
				this.manager = manager;
			}
				
			public string ID
			{
				get
				{
					return "cities-c" + countryId;
				}
			}

			public async Task<IEnumerable<ICity>> LocalQuery()
			{
				return await manager.Cache.FetchCities(countryId);
			}

			public async Task<IEnumerable<ICity>> RemoteQuery()
			{
				return await manager.Rest.FetchCities(countryId);
			}
		}

		private class FindProductsQuery : IQueryLoader<IProduct>
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

		private class FindStoresQuery : IQueryLoader<IStore>
		{
			int pageSize;
			int offset;
			IModelEntityManager manager;
			string query;

			public FindStoresQuery(string query, int pageSize, int offset, IModelEntityManager manager)
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
					string cachedQueryId = string.Format("stores-p{0}o{1}-{2}", pageSize, offset, query);
					return null;
				}
			}

			public async Task<IEnumerable<IStore>> LocalQuery()
			{
				return await manager.Cache.FindStores(query, pageSize, offset);
			}

			public async Task<IEnumerable<IStore>> RemoteQuery()
			{
				return await manager.Rest.FindStores(query, pageSize, offset);
			}
		}
		#endregion
	}
}


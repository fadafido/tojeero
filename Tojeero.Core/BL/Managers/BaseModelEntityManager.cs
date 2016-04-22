﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Contracts;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Core.Managers
{
    public class BaseModelEntityManager : IModelEntityManager
    {
        #region Private Fields and Properties

        #endregion

        #region Constructors

        public BaseModelEntityManager(ICacheRepository cacheRepository, IRestRepository restRepository)
        {
            Rest = restRepository;
            Cache = cacheRepository;
        }

        #endregion

        #region IModelEntityManager implementation

        public IRestRepository Rest { get; }

        public ICacheRepository Cache { get; }

        #endregion

        #region IModelEntityManager implementation

        public async Task<IEnumerable<T>> Fetch<T, Entity>(IQueryLoader<T> loader, double? expiresIn = null)
        {
            IEnumerable<T> result = null;
            var cacheName = CachedQuery.GetEntityCacheName<Entity>();
            var cachedQuery = await Cache.FetchObjectAsync<CachedQuery>(loader.ID).ConfigureAwait(false);
            var isExpired = cachedQuery == null || cachedQuery.IsExpired;

            //If the query has not ever been executed or was expired fetch the results from backend and save them to local cache
            if (isExpired)
            {
                result = await loader.RemoteQuery().ConfigureAwait(false);

                if (!string.IsNullOrEmpty(loader.ID))
                {
                    await loader.PostProcess(result);
                    await Cache.SaveAsync(result).ConfigureAwait(false);
                    cachedQuery = new CachedQuery
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

        #endregion
    }
}
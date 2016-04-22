using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers
{
    public class StoreCategoryManager : IStoreCategoryManager
    {
        #region Private fields and properties

        private readonly IModelEntityManager _manager;

        #endregion

        #region Constructors

        public StoreCategoryManager(IModelEntityManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region IStoreCategoryManager implementation

        public Task<IEnumerable<IStoreCategory>> Fetch()
        {
            return _manager.Fetch<IStoreCategory, StoreCategory>(new FetchStoreCategoriesQuery(_manager),
                Constants.StoresCacheTimespan.TotalMilliseconds);
        }

        public Task<Dictionary<string, int>> GetFacets(string query, IStoreFilter filter = null)
        {
            return _manager.Rest.GetStoreCategoryFacets(query, filter);
        }

        public async Task ClearCache()
        {
            await _manager.Cache.Clear<StoreCategory>();
        }

        #endregion
    }

    #region Queries

    public class FetchStoreCategoriesQuery : IQueryLoader<IStoreCategory>
    {
        readonly IModelEntityManager manager;

        public FetchStoreCategoriesQuery(IModelEntityManager manager)
        {
            this.manager = manager;
        }

        public string ID
        {
            get { return "storeCategories"; }
        }

        public async Task<IEnumerable<IStoreCategory>> LocalQuery()
        {
            return await manager.Cache.FetchStoreCategories();
        }

        public async Task<IEnumerable<IStoreCategory>> RemoteQuery()
        {
            return await manager.Rest.FetchStoreCategories();
        }

        public async Task PostProcess(IEnumerable<IStoreCategory> items)
        {
        }
    }

    #endregion
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Queries
{
    public class StoreProductsQueryLoader : IQueryLoader<IProduct>
    {
        readonly int pageSize;
        readonly int offset;
        readonly IModelEntityManager manager;
        readonly IStore store;
        readonly bool includeInvisible;

        public StoreProductsQueryLoader(int pageSize, int offset, IModelEntityManager manager, IStore store,
            bool includeInvisible = false)
        {
            this.includeInvisible = includeInvisible;
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
                return ToString();
            }
        }

        public async Task<IEnumerable<IProduct>> LocalQuery()
        {
            return await manager.Cache.FetchStoreProducts(store.ID, pageSize, offset, includeInvisible);
        }

        public async Task<IEnumerable<IProduct>> RemoteQuery()
        {
            return await manager.Rest.FetchStoreProducts(store.ID, pageSize, offset, includeInvisible);
        }

        public async Task PostProcess(IEnumerable<IProduct> items)
        {
        }

        public override string ToString()
        {
            var cachedQueryId = string.Format("store_products:p_{0}o_{1}-s_{2}", pageSize, offset, store.ID);
            return cachedQueryId;
        }
    }
}
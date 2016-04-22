using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Store
{
    public class FavoriteStoresViewModel : BaseCollectionViewModel<StoreViewModel>
    {
        #region Private fields and properties

        private readonly IStoreManager _manager;

        #endregion

        #region Constructors

        public FavoriteStoresViewModel(IStoreManager manager)
            : base(new FavoriteStoresQuery(manager), Constants.StoresPageSize)
        {
            _manager = manager;
            Placeholder = AppResources.MessageNoFavoriteStores;
        }

        #endregion

        #region Queries

        private class FavoriteStoresQuery : IModelQuery<StoreViewModel>
        {
            readonly IStoreManager manager;

            public FavoriteStoresQuery(IStoreManager manager)
            {
                this.manager = manager;
            }

            public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
            {
                var result = await manager.FetchFavorite(pageSize, offset);
                if (result == null)
                    return null;
                return result.Select(p => new StoreViewModel(p));
            }

            public Comparison<StoreViewModel> Comparer
            {
                get { return Comparers.StoreName; }
            }

            public Task ClearCache()
            {
                return manager.ClearCache();
            }
        }

        #endregion
    }
}
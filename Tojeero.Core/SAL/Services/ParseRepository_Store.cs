using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Algolia.Search;
using Parse;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.Services
{
    public partial class ParseRepository
    {
        #region IRepository implementation

        public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, IStoreFilter filter = null)
        {
            using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
            {
                var result = await FindStores("", pageSize, offset, filter);
                return result;
            }
        }

        public async Task<IEnumerable<IStore>> FetchFavoriteStores(int pageSize, int offset)
        {
            using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
            {
                var user = ParseUser.CurrentUser as TojeeroUser;
                if (user == null)
                    return null;
                var query = user.FavoriteStores.Query.Where(s => s.IsBlocked == false).OrderBy(p => p.LowercaseName);
                query = addStoreIncludedFields(query);
                if (pageSize > 0 && offset >= 0)
                {
                    query = query.Limit(pageSize).Skip(offset);
                }
                var result = await query.FindAsync();
                return result.Select(p => new Store(p) as IStore);
            }
        }

        public async Task<int> CountFavoriteStores()
        {
            var user = ParseUser.CurrentUser as TojeeroUser;
            if (user == null)
                return 0;
            var query = user.FavoriteStores.Query.Where(s => s.IsBlocked == false);
            var count = await query.CountAsync();
            return count;
        }

        public async Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset,
            IStoreFilter filter = null)
        {
            using (var tokenSource = new CancellationTokenSource(Constants.FindStoresTimeout))
            {
                var algoliaQuery = new Query(query);
                algoliaQuery = getFilteredStoreQuery(algoliaQuery, filter);
                if (pageSize > 0)
                {
                    algoliaQuery.SetNbHitsPerPage(pageSize);
                }
                if (offset > 0)
                {
                    algoliaQuery.SetPage(offset/pageSize);
                }
                var result = await _storeIndex.SearchAsync(algoliaQuery, tokenSource.Token);
                var stores = result["hits"].ToObject<List<Store>>();
                return stores;
            }
        }

        public async Task<int> CountStores(string query, IStoreFilter filter = null)
        {
            var algoliaQuery = new Query(query);
            algoliaQuery = getFilteredStoreQuery(algoliaQuery, filter);
            algoliaQuery.SetNbHitsPerPage(0);
            var result = await _storeIndex.SearchAsync(algoliaQuery);
            try
            {
                var count = result["nbHits"].ToObject<int>();
                return count;
            }
            catch
            {
            }
            return -1;
        }

        public async Task<IEnumerable<IProduct>> FetchStoreProducts(string storeID, int pageSize, int offset,
            bool includeInvisible = false)
        {
            using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
            {
                var store = ParseObject.CreateWithoutData<ParseStore>(storeID);
                var query = new ParseQuery<ParseProduct>().Where(p => p.Store == store).OrderBy(p => p.LowercaseName);
                query = addProductIncludedFields(query);
                if (!includeInvisible)
                {
                    query = addProductVisibilityConditions(query);
                }
                if (pageSize > 0 && offset >= 0)
                {
                    query = query.Limit(pageSize).Skip(offset);
                }
                var result = await query.FindAsync();
                return result.Select(p => new Product(p) as IProduct);
            }
        }

        public async Task<IStore> SaveStore(ISaveStoreViewModel store)
        {
            if (store == null)
                throw new NullReferenceException("When saving store the ISaveStoreViewModel should be non null");
            using (var tokenSource = new CancellationTokenSource(Constants.SaveStoreTimeout))
            {
                var s = store.CurrentStore != null ? store.CurrentStore : new Store();
                s.CategoryID = store.Category != null ? store.Category.ID : null;
                s.CountryId = store.Country != null ? store.Country.ID : null;
                s.CityId = store.City != null ? store.City.ID : null;
                s.Name = store.Name;
                s.Description = store.Description;
                s.DeliveryNotes = store.DeliveryNotes;
                s.OwnerID = ParseUser.CurrentUser.ObjectId;
                s.LowercaseName = s.Name.ToLower();
                s.SearchTokens = new[] {s.Name}.Tokenize();
                if (store.MainImage.NewImage != null)
                {
                    await s.SetMainImage(store.MainImage.NewImage);
                }
                await s.Save();
                return s;
            }
        }

        public async Task<IStore> FetchDefaultStoreForUser(string userID)
        {
            if (string.IsNullOrEmpty(userID))
                return null;
            var user = ParseObject.CreateWithoutData<TojeeroUser>(userID);
            var query = new ParseQuery<ParseStore>().Where(s => s.Owner == user);
            query = addStoreIncludedFields(query);
            var store = await query.FirstOrDefaultAsync();

            return store != null ? new Store(store) : null;
        }

        public async Task<bool> CheckStoreNameIsReserved(string storeName, string currentStoreID = null)
        {
            using (var tokenSource = new CancellationTokenSource(Constants.DefaultTimeout))
            {
                var query =
                    new ParseQuery<ReservedName>().Where(
                        r => r.Name == storeName && r.Type == (int) ReservedNameType.Store);
                var result = await query.FirstOrDefaultAsync(tokenSource.Token).ConfigureAwait(false);
                if (result != null)
                    return true;
                var storeQuery = new ParseQuery<ParseStore>().Where(s => s.LowercaseName == storeName)
                    .Select("objectId");
                if (!string.IsNullOrEmpty(currentStoreID))
                {
                    storeQuery = storeQuery.Where(s => s.ObjectId != currentStoreID);
                }
                var store = await storeQuery.FirstOrDefaultAsync(tokenSource.Token).ConfigureAwait(false);
                if (store != null)
                    return true;
                return false;
            }
        }

        public async Task<Dictionary<string, int>> GetStoreCategoryFacets(string query, IStoreFilter filter = null)
        {
            var result = await getStoreAttributeFacets(query, "categoryID", filter);
            return result;
        }

        public async Task<Dictionary<string, int>> GetStoreCountryFacets(string query, IStoreFilter filter = null)
        {
            var result = await getStoreAttributeFacets(query, "countryID", filter, "cityID");
            return result;
        }

        public async Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null)
        {
            var result = await getStoreAttributeFacets(query, "cityID", filter);
            return result;
        }

        #endregion

        #region Utility methods

        private ParseQuery<ParseStore> getFilteredStoreQuery(ParseQuery<ParseStore> query, IStoreFilter filter)
        {
            if (filter != null)
            {
                if (filter.Category != null)
                {
                    query =
                        query.Where(
                            s => s.Category == ParseObject.CreateWithoutData<ParseStoreCategory>(filter.Category.ID));
                }

                if (filter.Country != null)
                {
                    query = query.Where(s => s.Country == ParseObject.CreateWithoutData<ParseCountry>(filter.Country.ID));
                }

                if (filter.City != null)
                {
                    query = query.Where(s => s.City == ParseObject.CreateWithoutData<ParseCity>(filter.City.ID));
                }

                if (filter.Tags != null && filter.Tags.Count > 0)
                {
                    query = getContainsAllQuery(query, "tags", filter.Tags);
                }
            }
            return query;
        }

        private Query getFilteredStoreQuery(Query query, IStoreFilter filter, params string[] excludeFacets)
        {
            if (filter != null)
            {
                var facets = new List<string>();
                facets.Add("isBlocked:false");
                if (filter.Category != null && !excludeFacets.Contains("categoryID"))
                {
                    facets.Add("categoryID:" + filter.Category.ID);
                }

                if (filter.Country != null && !excludeFacets.Contains("countryID"))
                {
                    facets.Add("countryID:" + filter.Country.ID);
                }

                if (filter.City != null && !excludeFacets.Contains("cityID"))
                {
                    facets.Add("cityID:" + filter.City.ID);
                }

                if (facets.Count > 0)
                    query.SetFacetFilters(facets);

                if (filter.Tags != null && filter.Tags.Count > 0)
                {
                    query.SetTagFilters(string.Join(",", filter.Tags));
                }
            }
            return query;
        }

        private async Task<Dictionary<string, int>> getStoreAttributeFacets(string query, string facetAttribute,
            IStoreFilter filter = null, params string[] childFacets)
        {
            var algoliaQuery = new Query(query);
            algoliaQuery = getFilteredStoreQuery(algoliaQuery, filter, new[] {facetAttribute}.Concatenate(childFacets));
            algoliaQuery.SetNbHitsPerPage(0);
            algoliaQuery.SetFacets(new[] {facetAttribute});
            var result = await _storeIndex.SearchAsync(algoliaQuery);
            try
            {
                var facets = result["facets"][facetAttribute].ToObject<Dictionary<string, int>>();
                return facets;
            }
            catch
            {
            }
            return null;
        }

        ParseQuery<ParseStore> addStoreIncludedFields(ParseQuery<ParseStore> query)
        {
            var result = query.Include("category").Include("country").Include("city");
            return result;
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers
{
    public class CityManager : ICityManager
    {
        #region Private fields and properties

        private readonly IModelEntityManager _manager;

        #endregion

        #region Constructors

        public CityManager(IModelEntityManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region ICityManager implementation

        public Task<IEnumerable<ICity>> Fetch(string countryId)
        {
            return _manager.Fetch<ICity, City>(new FetchCitiesQuery(countryId, _manager),
                Constants.StoresCacheTimespan.TotalMilliseconds);
        }

        public Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null)
        {
            return _manager.Rest.GetProductCityFacets(query, filter);
        }

        public Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null)
        {
            return _manager.Rest.GetStoreCityFacets(query, filter);
        }

        public Task ClearCache()
        {
            return _manager.Cache.Clear<City>();
        }

        public ICity Create()
        {
            return new City();
        }

        #endregion
    }

    #region Queries

    public class FetchCitiesQuery : IQueryLoader<ICity>
    {
        readonly IModelEntityManager manager;
        readonly string countryId;

        public FetchCitiesQuery(string countryId, IModelEntityManager manager)
        {
            this.countryId = countryId;
            this.manager = manager;
        }

        public string ID
        {
            get { return "cities-c" + countryId; }
        }

        public async Task<IEnumerable<ICity>> LocalQuery()
        {
            return await manager.Cache.FetchCities(countryId);
        }

        public async Task<IEnumerable<ICity>> RemoteQuery()
        {
            return await manager.Rest.FetchCities(countryId);
        }

        public async Task PostProcess(IEnumerable<ICity> items)
        {
        }
    }

    #endregion
}
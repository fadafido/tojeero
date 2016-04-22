using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers
{
    public class CountryManager : ICountryManager
    {
        #region Private fields and properties

        private readonly IModelEntityManager _manager;
        private readonly AsyncReaderWriterLock _loadCountiesLock = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public CountryManager(IModelEntityManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region ICountryManager implementation

        public Task<IEnumerable<ICountry>> Fetch()
        {
            return _manager.Fetch<ICountry, Country>(new FetchCountriesQuery(_manager),
                Constants.StoresCacheTimespan.TotalMilliseconds);
        }

        public Task ClearCache()
        {
            return _manager.Cache.Clear<Country>();
        }

        public ICountry Create()
        {
            return new Country();
        }

        public Dictionary<string, ICountry> Countries { get; private set; }

        public async Task LoadCountries()
        {
            using (var writerLock = await _loadCountiesLock.WriterLockAsync())
            {
                if (Countries != null)
                    return;
                var countries = await Fetch();
                Countries = countries.ToDictionary(c => c.ID, c => c);
            }
        }


        public Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null)
        {
            return _manager.Rest.GetProductCountryFacets(query, filter);
        }

        public Task<Dictionary<string, int>> GetStoreCountryFacets(string query, IStoreFilter filter = null)
        {
            return _manager.Rest.GetStoreCountryFacets(query, filter);
        }

        #endregion
    }

    #region Queries

    public class FetchCountriesQuery : IQueryLoader<ICountry>
    {
        readonly IModelEntityManager manager;

        public FetchCountriesQuery(IModelEntityManager manager)
        {
            this.manager = manager;
        }

        public string ID
        {
            get { return "countries"; }
        }

        public async Task<IEnumerable<ICountry>> LocalQuery()
        {
            return await manager.Cache.FetchCountries();
        }

        public async Task<IEnumerable<ICountry>> RemoteQuery()
        {
            return await manager.Rest.FetchCountries();
        }

        public async Task PostProcess(IEnumerable<ICountry> items)
        {
        }
    }

    #endregion
}
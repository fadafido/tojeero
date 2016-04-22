using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface ICountryManager : IBaseModelEntityManager
    {
        Task<IEnumerable<ICountry>> Fetch();
        Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null);
        Task<Dictionary<string, int>> GetStoreCountryFacets(string query, IStoreFilter filter = null);
        ICountry Create();
        Dictionary<string, ICountry> Countries { get; }
        Task LoadCountries();
    }
}
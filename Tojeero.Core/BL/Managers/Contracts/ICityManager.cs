using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface ICityManager : IBaseModelEntityManager
    {
        Task<IEnumerable<ICity>> Fetch(string countryId);
        Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null);
        Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null);
        ICity Create();
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IStoreCategoryManager : IBaseModelEntityManager
    {
        Task<IEnumerable<IStoreCategory>> Fetch();
        Task<Dictionary<string, int>> GetFacets(string query, IStoreFilter filter = null);
    }
}
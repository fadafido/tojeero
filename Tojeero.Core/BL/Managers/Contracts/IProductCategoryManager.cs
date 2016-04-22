using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IProductCategoryManager : IBaseModelEntityManager
    {
        Task<IEnumerable<IProductCategory>> Fetch();
        Task<Dictionary<string, int>> GetFacets(string query, IProductFilter filter = null);
        IProductCategory Create();
    }
}
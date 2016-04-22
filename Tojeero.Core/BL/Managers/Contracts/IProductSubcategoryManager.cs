using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IProductSubcategoryManager : IBaseModelEntityManager
    {
        Task<IEnumerable<IProductSubcategory>> Fetch(string categoryID);
        Task<Dictionary<string, int>> GetFacets(string query, IProductFilter filter = null);
        IProductSubcategory Create();
    }
}
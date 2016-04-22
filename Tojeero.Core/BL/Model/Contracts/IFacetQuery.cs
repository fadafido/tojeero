using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core.Model.Contracts
{
    public interface IFacetQuery<T> where T : IUniqueEntity
    {
        Task<IEnumerable<T>> FetchObjects();
        Task<Dictionary<string, int>> FetchFacets();
    }
}
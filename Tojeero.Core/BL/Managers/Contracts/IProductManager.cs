using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IProductManager : IBaseModelEntityManager
    {
        Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset, IProductFilter filter = null);
        Task<IProduct> FetchProduct(string productID);
        Task<IEnumerable<IProduct>> FetchFavorite(int pageSize, int offset);
        Task<int> CountFavorite();
        Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset, IProductFilter filter = null);
        Task<int> Count(string query, IProductFilter filter = null);
        Task<IProduct> Save(ISaveProductViewModel store);
    }
}
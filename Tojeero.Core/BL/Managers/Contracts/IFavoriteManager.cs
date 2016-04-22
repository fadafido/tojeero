using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IFavoriteManager
    {
        IList<IFavorite> Favorites { get; }
        Task<IFavorite> GetStoreFavorite(string storeID);
        Task<IFavorite> GetProductFavorite(string productID);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IFavoriteManager
	{
		IList<IFavorite> Favorites { get; }
		Task<IFavorite> GetStoreFavorite(string storeID);
		Task<IFavorite> GetProductFavorite(string productID);
	}
}


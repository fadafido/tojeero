using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset, IProductFilter filter = null);
		Task<IEnumerable<IProduct>> FetchFavoriteProducts(int pageSize, int offset);
		Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset, IProductFilter filter = null);
	}
}


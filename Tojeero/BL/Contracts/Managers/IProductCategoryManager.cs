using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductCategoryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IProductCategory>> Fetch();
		Task<Dictionary<string, int>> GetFacets(string query, IProductFilter filter = null);
		IProductCategory Create();
	}
}


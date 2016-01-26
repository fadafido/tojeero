using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductSubcategoryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IProductSubcategory>> Fetch(string categoryID);
		Task<Dictionary<string, int>> GetFacets(string query, IProductFilter filter = null);
		IProductSubcategory Create();
	}
}


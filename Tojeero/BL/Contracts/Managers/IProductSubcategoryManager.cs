using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductSubcategoryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IProductSubcategory>> Fetch(string categoryID);
		IProductSubcategory Create();
	}
}


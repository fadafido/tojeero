using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{ 
	public interface ICityManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICity>> Fetch(string countryId);
		Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null);
		ICity Create();
	}
}


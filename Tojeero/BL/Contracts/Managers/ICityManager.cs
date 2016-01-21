using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{ 
	public interface ICityManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICity>> Fetch(string countryId);
		Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null);
		ICity Create();
	}
}


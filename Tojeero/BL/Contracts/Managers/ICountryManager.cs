using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface ICountryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICountry>> Fetch();
		Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null);
		ICountry Create();
		Dictionary<string, ICountry> Countries { get; }
		Task LoadCountries();
	}
}


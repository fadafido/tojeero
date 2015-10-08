using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface ICountryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICountry>> FetchCountries();
	}
}


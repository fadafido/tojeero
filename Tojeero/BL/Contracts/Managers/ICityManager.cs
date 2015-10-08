using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{ 
	public interface ICityManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICity>> FetchCities(int countryId);
	}
}


using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{ 
	public interface ICityManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ICity>> Fetch(int countryId);
		ICity Create();
	}
}


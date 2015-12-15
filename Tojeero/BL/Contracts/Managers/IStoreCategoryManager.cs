using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IStoreCategoryManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IStoreCategory>> Fetch();
	}
}


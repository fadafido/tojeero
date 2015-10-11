using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public interface IStoreManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset);
		Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset);
	}
}


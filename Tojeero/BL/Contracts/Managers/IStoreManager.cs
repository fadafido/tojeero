using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public interface IStoreManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IStore>> Fetch(int pageSize, int offset, IStoreFilter filter = null);
		Task<IEnumerable<IStore>> Find(string query, int pageSize, int offset, IStoreFilter filter = null);
	}
}


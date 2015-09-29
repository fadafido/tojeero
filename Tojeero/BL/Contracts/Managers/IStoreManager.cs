using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public interface IStoreManager
	{
		Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, CancellationToken token);
	}
}


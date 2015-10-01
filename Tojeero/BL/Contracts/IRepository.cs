using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tojeero.Core;
using System.Threading;

namespace Tojeero.Core
{
	public interface IRepository
	{
		Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset);
		Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset, CancellationToken token);

//		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset);
//		Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset);
	}
}


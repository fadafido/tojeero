using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductManager
	{
		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset);
	}
}


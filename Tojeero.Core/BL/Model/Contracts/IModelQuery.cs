using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core.Model.Contracts
{
	public interface IModelQuery<T>
	{
		Task<IEnumerable<T>> Fetch(int pageSize = - 1, int offset = -1);
		Comparison<T> Comparer { get; }
		Task ClearCache();
	}
	
}

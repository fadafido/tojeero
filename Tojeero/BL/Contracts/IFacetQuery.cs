using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IFacetQuery<T> where T : IUniqueEntity
	{
		Task<IEnumerable<T>> FetchObjects();
		Task<Dictionary<string, int>> FetchFacets();
	}
}


using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface ITagManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ITag>> Fetch(int pageSize, int offset);
		Task<IEnumerable<ITag>> Find(string searchQuery, int pageSize, int offset);
	}
}


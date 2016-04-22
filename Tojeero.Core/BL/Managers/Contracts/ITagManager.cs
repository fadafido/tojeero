using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
	public interface ITagManager : IBaseModelEntityManager
	{
		Task<IEnumerable<ITag>> Fetch(int pageSize, int offset);
		Task<IEnumerable<ITag>> Find(string searchQuery, int pageSize, int offset);
	}
}


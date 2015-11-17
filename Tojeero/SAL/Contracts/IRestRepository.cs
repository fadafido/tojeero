using System;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IRestRepository : IRepository
	{
		Task<IStore> FetchDefaultStoreForUser(string userID);
	}
}


using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
	public interface IStoreManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IStore>> Fetch(int pageSize, int offset, IStoreFilter filter = null);
		Task<IEnumerable<IStore>> FetchFavorite(int pageSize, int offset);
		Task<int> CountFavorite();
		Task<IEnumerable<IStore>> Find(string query, int pageSize, int offset, IStoreFilter filter = null);
		Task<int> Count(string query, IStoreFilter filter = null);
		Task<IStore> Save(ISaveStoreViewModel store);
		Task<bool> CheckNameIsReserved(string storeName, string currentStoreID = null);
	}
}


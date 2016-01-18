using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core
{
	public interface IStoreManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IStore>> Fetch(int pageSize, int offset, IStoreFilter filter = null);
		Task<IEnumerable<IStore>> FetchFavorite(int pageSize, int offset);
		Task<int> CountFavorite();
		Task<IEnumerable<IStore>> Find(string query, int pageSize, int offset, IStoreFilter filter = null);
		Task<IStore> Save(ISaveStoreViewModel store);
		Task<bool> CheckNameIsReserved(string storeName, string currentStoreID = null);
	}
}


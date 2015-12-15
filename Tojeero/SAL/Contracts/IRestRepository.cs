using System;
using System.Threading.Tasks;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core
{
	public interface IRestRepository : IRepository
	{
		Task<IStore> FetchDefaultStoreForUser(string userID);
		Task<bool> CheckStoreNameIsReserved(string storeName, string currentStoreID = null);

		Task<IStore> SaveStore(ISaveStoreViewModel store);
		Task<IProduct> SaveProduct(ISaveProductViewModel product);
	}
}


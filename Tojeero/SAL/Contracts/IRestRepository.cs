using System;
using System.Threading.Tasks;
using Tojeero.Core.ViewModels;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IRestRepository : IRepository
	{
		Task<IStore> FetchDefaultStoreForUser(string userID);
		Task<bool> CheckStoreNameIsReserved(string storeName, string currentStoreID = null);
		Task<int> CountFavoriteProducts();

		Task<IStore> SaveStore(ISaveStoreViewModel store);
		Task<IProduct> SaveProduct(ISaveProductViewModel product);
		Task<int> CountFavoriteStores();

		//PRODUCT FACETING
		Task<Dictionary<string, int>> GetProductCategoryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductSubcategoryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null);

		//STORE FACETING
		Task<Dictionary<string, int>> GetStoreCategoryFacets(string query, IStoreFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCountryFacets(string query, IStoreFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null);
	}
}


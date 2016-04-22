﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.Services.Contracts
{
	public interface IRestRepository : IRepository
	{
        //STORE
		Task<IStore> FetchDefaultStoreForUser(string userID);
		Task<bool> CheckStoreNameIsReserved(string storeName, string currentStoreID = null);
        Task<int> CountFavoriteStores();

        //PRODUCT
        Task<IStore> SaveStore(ISaveStoreViewModel store);
		Task<IProduct> SaveProduct(ISaveProductViewModel product);
        Task<int> CountFavoriteProducts();
        Task<IProduct> FetchProduct(string productID);

        //PRODUCT FACETING
        Task<int> CountProducts(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductCategoryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductSubcategoryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null);
		Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null);

		//STORE FACETING
		Task<int> CountStores(string query, IStoreFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCategoryFacets(string query, IStoreFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCountryFacets(string query, IStoreFilter filter = null);
		Task<Dictionary<string, int>> GetStoreCityFacets(string query, IStoreFilter filter = null);
	}
}

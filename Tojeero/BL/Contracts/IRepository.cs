using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tojeero.Core;
using System.Threading;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core
{
	public interface IRepository
	{
		//Products
		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, IProductFilter filter = null);
		Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset, IProductFilter filter = null);
		Task<IEnumerable<IProduct>> FetchFavoriteProducts(int pageSize, int offset);

		//Product categories
		Task<IEnumerable<IProductCategory>> FetchProductCategories();
		Task<IEnumerable<IProductSubcategory>> FetchProductSubcategories(string categoryID);

		//Stores
		Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, IStoreFilter filter = null);
		Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset, IStoreFilter filter = null);
		Task<IEnumerable<IStore>> FetchFavoriteStores(int pageSize, int offset);
		Task<IEnumerable<IProduct>> FetchStoreProducts(string storeID, int pageSize, int offset);
		Task SaveStore(IStoreViewModel store);

		//Store categories
		Task<IEnumerable<IStoreCategory>> FetchStoreCategories();

		//Countries
		Task<IEnumerable<ICountry>> FetchCountries();

		//Cities
		Task<IEnumerable<ICity>> FetchCities(string countryId);

		//Tags
		Task<IEnumerable<ITag>> FetchTags(int pageSize, int offset);
		Task<IEnumerable<ITag>> FindTags(string searchQuery, int pageSize, int offset);
	}
}


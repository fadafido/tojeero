using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tojeero.Core;
using System.Threading;

namespace Tojeero.Core
{
	public interface IRepository
	{
		//Products
		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset);
		Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset);

		//Product categories
		Task<IEnumerable<IProductCategory>> FetchProductCategories();
		Task<IEnumerable<IProductSubcategory>> FetchProductSubcategories(string categoryID);

		//Stores
		Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset);
		Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset);

		//Store categories
		Task<IEnumerable<IStoreCategory>> FetchStoreCategories();

		//Countries
		Task<IEnumerable<ICountry>> FetchCountries();

		//Cities
		Task<IEnumerable<ICity>> FetchCities(int countryId);
	}
}


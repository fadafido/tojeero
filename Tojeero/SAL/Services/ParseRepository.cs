using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.Toolbox;
using System.Collections;

namespace Tojeero.Core
{
	public class ParseRepository : IRestRepository
	{
		#region Constructors

		public ParseRepository()
		{
		}

		#endregion

		#region IRepository implementation

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, IProductFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
 				var query = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName);
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				query = getFilteredProductQuery(query, filter);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, IStoreFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var query = new ParseQuery<ParseStore>().OrderBy(s => s.LowercaseName).Include("category");
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				query = getFilteredStoreQuery(query, filter);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Store(s) as IStore);
			}
		}


		public async Task<IEnumerable<ICountry>> FetchCountries()
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchCountriesTimeout))
			{
				var query = new ParseQuery<ParseCountry>();
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new Country(c) as ICountry);
			}
		}

		public async Task<IEnumerable<ICity>> FetchCities(int countryId)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchCitiesTimeout))
			{
				var query = new ParseQuery<ParseCity>().Where(c => c.CountryId == countryId);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new City(c) as ICity);
			}
		}
			
		public async Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset, IProductFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindProductsTimeout))
			{				
				var parseQuery = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Count > 0)
				{
					parseQuery = getContainsAllQuery(parseQuery, "searchTokens", tokens);
				}
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				parseQuery = getFilteredProductQuery(parseQuery, filter);
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset, IStoreFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindStoresTimeout))
			{
				var parseQuery = new ParseQuery<ParseStore>().OrderBy(s => s.LowercaseName);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Count > 0)
				{
					parseQuery = getContainsAllQuery(parseQuery, "searchTokens", tokens);
				}
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				parseQuery = getFilteredStoreQuery(parseQuery, filter);
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Store(s) as IStore);
			}
		}

		public async Task<IEnumerable<IProductCategory>> FetchProductCategories()
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductSubcategoriesTimeout))
			{
				var query = new ParseQuery<ParseProductCategory>();
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new ProductCategory(c) as IProductCategory);
			}
		}

		public async Task<IEnumerable<IProductSubcategory>> FetchProductSubcategories(string categoryID)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductSubcategoriesTimeout))
			{
				var query = new ParseQuery<ParseProductSubcategory>().Where(sub => sub.Category == ParseObject.CreateWithoutData<ParseProductCategory>(categoryID));
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new ProductSubcategory(c) as IProductSubcategory);
			}
		}

		public async Task<IEnumerable<IStoreCategory>> FetchStoreCategories()
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoreSubcategoriesTimeout))
			{
				var query = new ParseQuery<ParseStoreCategory>();
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new StoreCategory(c) as IStoreCategory);
			}
		}

		public async Task<IEnumerable<ITag>> FetchTags(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchTagsTimeout))
			{
				var query = new ParseQuery<ParseTag>().OrderBy(p => p.Text);
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Tag(p) as ITag);
			}
		}

		public async Task<IEnumerable<ITag>> FindTags(string searchQuery, int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindTagsTimeout))
			{
				var parseQuery = new ParseQuery<ParseTag>().Where(t => t.Text.StartsWith(searchQuery.Trim())).OrderBy(t => t.Text);
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Tag(s) as ITag);
			}
		}
		#endregion

		#region Utility methods

		private ParseQuery<ParseProduct> getFilteredProductQuery(ParseQuery<ParseProduct> query, IProductFilter filter)
		{
			if (filter != null)
			{
				if (filter.Category != null)
				{
					query = query.Where(p => p.Category == ParseObject.CreateWithoutData<ParseProductCategory>(filter.Category.ID));
				}

				if (filter.Subcategory != null)
				{
					query = query.Where(p => p.Subcategory == ParseObject.CreateWithoutData<ParseProductSubcategory>(filter.Subcategory.ID));
				}

				if (filter.Country != null)
				{
					query = query.Where(p => p.CountryId == filter.Country.CountryId);
				}

				if (filter.City != null)
				{
					query = query.Where(p => p.CityId == filter.City.CityId);
				}

				if (filter.Tags != null && filter.Tags.Count > 0)
				{
					query = getContainsAllQuery(query, "tags", filter.Tags);
				}

				if (filter.StartPrice != null)
				{
					query = query.Where(p => p.Price >= filter.StartPrice.Value);
				}

				if (filter.EndPrice != null)
				{
					query = query.Where(p => p.Price <= filter.EndPrice.Value);
				}
			}
			return query;
		}

		private ParseQuery<ParseStore> getFilteredStoreQuery(ParseQuery<ParseStore> query, IStoreFilter filter)
		{
			if (filter != null)
			{
				if (filter.Category != null)
				{
					query = query.Where(s => s.Category == ParseObject.CreateWithoutData<ParseStoreCategory>(filter.Category.ID));
				}

				if (filter.Country != null)
				{
					query = query.Where(s => s.CountryId == filter.Country.CountryId);
				}

				if (filter.City != null)
				{
					query = query.Where(s => s.CityId == filter.City.CityId);
				}

				if (filter.Tags != null && filter.Tags.Count > 0)
				{
					query = getContainsAllQuery(query, "tags", filter.Tags);
				}
			}
			return query;
		}

		private ParseQuery<T> getContainsAllQuery<T, ItemType>(ParseQuery<T> query, string propertyName, IList<ItemType> items) where T : ParseObject
		{
			var newItems = items.Count <= Constants.ParseContainsAllLimit ? items : items.SubCollection(0, Constants.ParseContainsAllLimit);
			query = query.WhereContainsAll(propertyName, newItems);
			return query;
		}
		#endregion
	}
}


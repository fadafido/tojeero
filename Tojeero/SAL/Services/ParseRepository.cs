using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.Toolbox;

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

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
				var query = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName);
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var query = new ParseQuery<ParseStore>().OrderBy(s => s.LowercaseName);
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
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
			
		public async Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindProductsTimeout))
			{				
				var parseQuery = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Count > 0)
				{
					parseQuery = parseQuery.WhereContainsAll("searchTokens", tokens);
				}
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindStoresTimeout))
			{
				var parseQuery = new ParseQuery<ParseStore>().OrderBy(s => s.LowercaseName);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Count > 0)
				{
					parseQuery = parseQuery.WhereContainsAll("searchTokens", tokens);
				}
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
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

	}
}


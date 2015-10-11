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
				var query = new ParseQuery<ParseProduct>().OrderBy(p => p.Name);
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
				var query = new ParseQuery<ParseStore>().OrderBy(s => s.Name);
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
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{				
				var parseQuery = new ParseQuery<ParseProduct>().OrderBy(p => p.Name);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Length > 0)
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
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var parseQuery = new ParseQuery<ParseStore>().OrderBy(s => s.Name);
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Length > 0)
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
		#endregion

	}
}


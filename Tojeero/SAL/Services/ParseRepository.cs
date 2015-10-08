using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

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
				var query = new ParseQuery<ParseProduct>().Limit(pageSize).Skip(offset);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var query = new ParseQuery<ParseStore>().Limit(pageSize).Skip(offset);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Store(s) as IStore);
			}
		}


		public async Task<IEnumerable<ICountry>> FetchCountries()
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchCountriesTimeout))
			{
				var query = new ParseQuery<ParseCountry>().OrderBy(c => c.CountryId);
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

		#endregion

	}
}


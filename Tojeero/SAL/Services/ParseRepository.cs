using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.Toolbox;
using System.Collections;
using Algolia.Search;
using Cirrious.CrossCore;
using System.Net.Http;
using ModernHttpClient;

namespace Tojeero.Core
{
	public partial class ParseRepository : IRestRepository
	{
		#region Private fields and properties

		private readonly AlgoliaClient _algoliaClient;
		private readonly Index _storeIndex;
		private readonly Index _productIndex;

		#endregion

		#region Constructors

		public ParseRepository()
		{
			_algoliaClient = new AlgoliaClient(Constants.AlgoliaApplicationId, Constants.AlgoliaSecurityKey, null, new NativeMessageHandler());
			_storeIndex = _algoliaClient.InitIndex(Constants.StoreIndex);
			_productIndex = _algoliaClient.InitIndex(Constants.ProductIndex);
		}

		#endregion

		#region IRepository implementation

		public async Task<IEnumerable<ICountry>> FetchCountries()
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchCountriesTimeout))
			{
				var query = new ParseQuery<ParseCountry>();
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new Country(c) as ICountry);
			}
		}

		public async Task<IEnumerable<ICity>> FetchCities(string countryId)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchCitiesTimeout))
			{
				var query = new ParseQuery<ParseCity>().Where(c => c.Country == ParseObject.CreateWithoutData<ParseCountry>(countryId));
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(c => new City(c) as ICity);
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

		#endregion

		#region Utility methods

		private ParseQuery<T> getContainsAllQuery<T, ItemType>(ParseQuery<T> query, string propertyName, IList<ItemType> items) where T : ParseObject
		{
			var newItems = items.Count <= Constants.ParseContainsAllLimit ? items : items.SubCollection(0, Constants.ParseContainsAllLimit);
			query = query.WhereContainsAll(propertyName, newItems);
			return query;
		}

		#endregion
	}
}


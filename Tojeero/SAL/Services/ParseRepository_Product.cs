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
	public partial class ParseRepository
	{
		#region IRepository implementation

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, IProductFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
				var query = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName).Include("category").Include("subcategory");
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				query = getFilteredProductQuery(query, filter);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IProduct>> FetchFavoriteProducts(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				if (user == null)
					return null;
				var query = user.FavoriteProducts.Query.OrderBy(p => p.LowercaseName).Include("category").Include("subcategory");
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync();	
				return result.Select(p => new Product(p) as IProduct);
			}
		}

		public async Task<IEnumerable<IProduct>> FindProducts(string query, int pageSize, int offset, IProductFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindProductsTimeout))
			{				
				var parseQuery = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName).Include("category").Include("subcategory");
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
					query = query.Where(p => p.Country == ParseObject.CreateWithoutData<ParseCountry>(filter.Country.ID));
				}

				if (filter.City != null)
				{
					query = query.Where(p => p.City == ParseObject.CreateWithoutData<ParseCity>(filter.City.ID));
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
			
		#endregion
	}
}


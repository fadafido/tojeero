using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.Toolbox;
using System.Collections;
using Tojeero.Core.ViewModels;
using Newtonsoft.Json;

namespace Tojeero.Core
{
	public partial class ParseRepository
	{
		#region IRepository implementation

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, IProductFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
				var query = new ParseQuery<ParseProduct>().OrderBy(p => p.LowercaseName);
				query = addProductVisibilityConditions(query);
				query = addProductIncludedFields(query);
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
				var query = user.FavoriteProducts.Query.OrderBy(p => p.LowercaseName);
				query = addProductVisibilityConditions(query);
				query = addProductIncludedFields(query);
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
				var algoliaQuery = new Algolia.Search.Query(query);
				algoliaQuery = getFilteredProductQuery(algoliaQuery, filter);
				if (pageSize > 0)
				{
					algoliaQuery.SetNbHitsPerPage(pageSize);
				}
				if (offset > 0)
				{
					algoliaQuery.SetPage(offset / pageSize);
				}

				var result = await _productIndex.SearchAsync(algoliaQuery, tokenSource.Token);
				var products = result["hits"].ToObject<List<Product>>();
				await GetProductCategoryFacets(query, filter);
				return products;
			}
		}

		public async Task<int> CountFavoriteProducts()
		{
			var user = ParseUser.CurrentUser as TojeeroUser;
			if (user == null)
				return 0;
			var query = user.FavoriteProducts.Query;
			query = addProductVisibilityConditions(query);
			var count = await query.CountAsync();
			return count;
		}

		public async Task<IProduct> SaveProduct(ISaveProductViewModel product)
		{
			if (product == null || product.Store == null)
				throw new NullReferenceException("When saving product the ISaveProductViewModel parameter as well as product's store should be non null");
			using (var tokenSource = new CancellationTokenSource(Constants.SaveProductTimeout))
			{
				var p = product.CurrentProduct != null ? product.CurrentProduct : new Product();
				p.Name = product.Name;
				p.Price = product.Price;
				p.StoreID = product.Store.ID;
				p.CategoryID = product.Category != null ? product.Category.ID : null;
				p.SubcategoryID = product.Subcategory != null ? product.Subcategory.ID : null;
				p.Description = product.Description;
				p.NotVisible = !product.Visible;
				p.Status = ProductStatus.Pending;
				p.LowercaseName = p.Name.ToLower();
				p.CountryId = product.Store.CountryId;
				p.CityId = product.Store.CityId;
				p.Tags = product.Tags.ToList();
				p.SearchTokens = new string[] { p.Name, p.TagList }.Tokenize();
				if (product.MainImage.NewImage != null)
				{
					await p.SetMainImage(product.MainImage.NewImage);
				}
				if (product.Images != null)
				{
					var images = product.Images.Where(i => i.NewImage != null).Select(i => i.NewImage);
					await p.AddImages(images, false);
				}
				await p.Save();
				return p;
			}
		}

		public async Task<Dictionary<string, int>> GetProductCategoryFacets(string query, IProductFilter filter = null)
		{
			var result = await getProductAttributeFacets(query, "categoryID", filter);
			return result;
		}

		public async Task<Dictionary<string, int>> GetProductSubcategoryFacets(string query, IProductFilter filter = null)
		{
			var result = await getProductAttributeFacets(query, "subcategoryID", filter);
			return result;
		}

		public async Task<Dictionary<string, int>> GetProductCountryFacets(string query, IProductFilter filter = null)
		{
			var result = await getProductAttributeFacets(query, "countryID", filter);
			return result;
		}

		public async Task<Dictionary<string, int>> GetProductCityFacets(string query, IProductFilter filter = null)
		{
			var result = await getProductAttributeFacets(query, "cityID", filter);
			return result;
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

		private Algolia.Search.Query getFilteredProductQuery(Algolia.Search.Query query, IProductFilter filter, params string[] excludeFacets )
		{
			if (filter != null)
			{
				List<string> facets = new List<string>();
				facets.Add("status:"+(int)ProductStatus.Approved);
				facets.Add("notVisible:false");
				facets.Add("isBlocked:false");
				if (filter.Category != null && !excludeFacets.Contains("categoryID"))
				{
					facets.Add("categoryID:"+filter.Category.ID);
				}

				if (filter.Subcategory != null && !excludeFacets.Contains("subcategoryID"))
				{
					facets.Add("subcategoryID:"+filter.Subcategory.ID);
				}

				if (filter.Country != null && !excludeFacets.Contains("countryID"))
				{
					facets.Add("countryID:"+filter.Country.ID);
				}

				if (filter.City != null && !excludeFacets.Contains("cityID"))
				{
					facets.Add("cityID:"+filter.City.ID);
				}

				if (facets.Count > 0)
					query.SetFacetFilters(facets);

				if (filter.Tags != null && filter.Tags.Count > 0)
				{
					query.SetTagFilters(string.Join(",", filter.Tags));
				}

				List<string> numericFilters = new List<string>();
				numericFilters.Add("status=" + (int)ProductStatus.Approved);

				if (filter.StartPrice != null)
				{
					numericFilters.Add("price>=" + filter.StartPrice);
				}

				if (filter.EndPrice != null)
				{
					numericFilters.Add("price<=" + filter.EndPrice);
				}

				if (numericFilters.Count > 0)
					query.SetNumericFilters(string.Join(",", numericFilters));
			}
			return query;
		}

		private async Task<Dictionary<string, int>> getProductAttributeFacets(string query, string facetAttribute, IProductFilter filter = null)
		{
			var algoliaQuery = new Algolia.Search.Query();
			algoliaQuery = getFilteredProductQuery(algoliaQuery, filter, facetAttribute);
			algoliaQuery.SetNbHitsPerPage(0);
			algoliaQuery.SetFacets(new string[] {facetAttribute});
			var result = await _productIndex.SearchAsync(algoliaQuery);
			var json = result.ToString();
			try
			{
				var facets = result["facets"][facetAttribute].ToObject<Dictionary<string, int>>();
				return facets;
			}
			catch
			{
			}
			return null;
		}

		ParseQuery<ParseProduct> addProductVisibilityConditions(ParseQuery<ParseProduct> query)
		{
			var result = query.Where(p => p.NotVisible == false && p.Status == (int)ProductStatus.Approved && p.IsBlocked == false);
			return result;
		}

		ParseQuery<ParseProduct> addProductIncludedFields(ParseQuery<ParseProduct> query)
		{
			var result = query.Include("category").Include("subcategory").Include("store").Include("country");
			return result;
		}

		#endregion
	}
}


using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;

namespace Tojeero.Core
{
	public class ProductSubcategoryManager : IProductSubcategoryManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public ProductSubcategoryManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IProductSubcategoryManager implementation

		public Task<IEnumerable<IProductSubcategory>> Fetch(string categoryID)
		{
			return _manager.Fetch<IProductSubcategory, ProductSubcategory>(new FetchProductSubcategoriesQuery(categoryID, _manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task<Dictionary<string, int>> GetFacets(string query, IProductFilter filter = null)
		{
			return _manager.Rest.GetProductSubcategoryFacets(query, filter);
		}

		public async Task ClearCache()
		{
			await _manager.Cache.Clear<ProductSubcategory>();
		}

		public IProductSubcategory Create()
		{
			return new ProductSubcategory();
		}

		#endregion
	}

	#region Queries

	public class FetchProductSubcategoriesQuery : IQueryLoader<IProductSubcategory>
	{
		IModelEntityManager manager;
		string categoryID;

		public FetchProductSubcategoriesQuery(string categoryID, IModelEntityManager manager)
		{
			this.categoryID = categoryID;
			this.manager = manager;
		}

		public string ID
		{
			get
			{
				return "productSubcategories-c"+categoryID;
			}
		}

		public async Task<IEnumerable<IProductSubcategory>> LocalQuery()
		{
			return await manager.Cache.FetchProductSubcategories(categoryID);
		}

		public async Task<IEnumerable<IProductSubcategory>> RemoteQuery()
		{
			return await manager.Rest.FetchProductSubcategories(categoryID);
		}

		public async Task PostProcess(IEnumerable<IProductSubcategory> items)
		{
		}
	}

	#endregion
}


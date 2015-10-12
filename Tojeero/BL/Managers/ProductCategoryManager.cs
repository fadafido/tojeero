﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;

namespace Tojeero.Core
{
	public class ProductCategoryManager : IProductCategoryManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public ProductCategoryManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IProductCategoryManager implementation

		public Task<IEnumerable<IProductCategory>> Fetch()
		{
			return _manager.Fetch<IProductCategory, ProductCategory>(new FetchProductCategoriesQuery(_manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public async Task ClearCache()
		{
			await _manager.Cache.Clear<ParseProductCategory>();
		}

		#endregion
	}

	#region Queries

	public class FetchProductCategoriesQuery : IQueryLoader<IProductCategory>
	{
		IModelEntityManager manager;

		public FetchProductCategoriesQuery(IModelEntityManager manager)
		{
			this.manager = manager;
		}

		public string ID
		{
			get
			{
				return "productCategories";
			}
		}

		public async Task<IEnumerable<IProductCategory>> LocalQuery()
		{
			return await manager.Cache.FetchProductCategories();
		}

		public async Task<IEnumerable<IProductCategory>> RemoteQuery()
		{
			return await manager.Rest.FetchProductCategories();
		}
	}

	#endregion
}


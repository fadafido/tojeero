using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;

namespace Tojeero.Core
{
	public class StoreCategoryManager : IStoreCategoryManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public StoreCategoryManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IStoreCategoryManager implementation

		public Task<IEnumerable<IStoreCategory>> Fetch()
		{
			return _manager.Fetch<IStoreCategory, StoreCategory>(new FetchStoreCategoriesQuery(_manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public async Task ClearCache()
		{
			await _manager.Cache.Clear<ParseStoreCategory>();
		}

		#endregion
	}

	#region Queries

	public class FetchStoreCategoriesQuery : IQueryLoader<IStoreCategory>
	{
		IModelEntityManager manager;

		public FetchStoreCategoriesQuery(IModelEntityManager manager)
		{
			this.manager = manager;
		}

		public string ID
		{
			get
			{
				return "storeCategories";
			}
		}

		public async Task<IEnumerable<IStoreCategory>> LocalQuery()
		{
			return await manager.Cache.FetchStoreCategories();
		}

		public async Task<IEnumerable<IStoreCategory>> RemoteQuery()
		{
			return await manager.Rest.FetchStoreCategories();
		}
	}

	#endregion
}


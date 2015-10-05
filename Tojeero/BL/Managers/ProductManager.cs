using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;
using System.Linq;

namespace Tojeero.Core
{
	public class ProductManager : IProductManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public ProductManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IProductManager implementation

		public Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			return _manager.FetchProducts(pageSize, offset);
		}


		public Task ClearCache()
		{
			return _manager.Cache.Clear<Product>(Constants.ProductsCacheName);
		}

		#endregion
	}
}


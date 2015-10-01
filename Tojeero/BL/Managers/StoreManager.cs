using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;
using System.Linq;

namespace Tojeero.Core
{
	public class StoreManager : IStoreManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public StoreManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region IStoreManager Implementation

		public Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			return _manager.FetchStores(pageSize, offset);
		}

		#endregion
	}
}


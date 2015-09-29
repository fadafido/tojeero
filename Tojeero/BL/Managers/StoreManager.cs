using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Parse;

namespace Tojeero.Core
{
	public class StoreManager : IStoreManager
	{
		#region Constructors

		public StoreManager()
			:base()
		{
		}

		#endregion

		#region IStoreManager Implementation

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, CancellationToken token)
		{
			var query = new ParseQuery<Store>().Limit(pageSize).Skip(offset);
			var result = await query.FindAsync();
			return result;
		}

		#endregion
	}
}


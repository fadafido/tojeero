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
				var query = new ParseQuery<Product>().Limit(pageSize).Skip(offset);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Cast<IProduct>();;
			}
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var query = new ParseQuery<Store>().Limit(pageSize).Skip(offset);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Cast<IStore>();
			}
		}

		#endregion

	}
}


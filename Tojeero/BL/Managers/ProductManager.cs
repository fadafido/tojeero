using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parse;

namespace Tojeero.Core
{
	public class ProductManager : IProductManager
	{
		#region Constructors

		public ProductManager()
			: base()
		{
		}

		#endregion

		#region IProductManager implementation

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, CancellationToken token)
		{
			var query = new ParseQuery<Product>().Limit(pageSize).Skip(offset);
			var result = await query.FindAsync();
			return result;
		}

		#endregion
	}
}


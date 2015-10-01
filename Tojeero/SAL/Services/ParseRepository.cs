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

		public Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset)
		{
			return FetchAsync<T>(pageSize, offset, CancellationToken.None);
		}

		public async Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset, CancellationToken token)
		{
			var name = getParseTableName<T>();
			var query = ParseObject.GetQuery(name).Limit(pageSize).Skip(offset);
			var result = await query.FindAsync(token);
			return result.Cast<T>();
		}

		#endregion

		#region Utility Methods

		private string getParseTableName<T>()
		{
			var tableAttribute = IntrospectionExtensions.GetTypeInfo(typeof(T)).GetCustomAttribute<ParseClassNameAttribute>();	
			if (tableAttribute == null)
				throw new InvalidOperationException("You can only send requests to Parse for entities which are decorated with ParseClassNameAttribute.");
			return tableAttribute;
		}

		#endregion
	}
}


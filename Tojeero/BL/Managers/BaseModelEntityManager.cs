using System;
using System.Linq;
using Parse;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class BaseModelEntityManager<EntityType> : IModelEntityManager<EntityType>
		where EntityType : BaseModelEntity
	{
		
		#region IModelEntityManager implementation

		public Task<EntityType> FetchAsync(string id)
		{
			return FetchAsync(id, CancellationToken.None);
		}

		public async Task<EntityType> FetchAsync(string id, CancellationToken token)
		{
			var query = from entity in new ParseQuery<EntityType>()
			            where entity.ObjectId == id
			            select entity;
			var result = await query.FindAsync(token);
			return result.FirstOrDefault();
		}

		public Task<List<EntityType>> FetchAsync(int offset, int count)
		{
			return FetchAsync(offset, count, CancellationToken.None);
		}

		public async Task<List<EntityType>> FetchAsync(int offset, int count, CancellationToken token)
		{
			var query = new ParseQuery<EntityType>().Skip(offset).Limit(count);
			var result = await query.FindAsync(token);
			return result.ToList();
		}

		#endregion
		
	}
}


using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public class BaseModelEntityManager<EntityType> : IModelEntityManager<EntityType>
		where EntityType : IModelEntity
	{
		
		#region IModelEntityManager implementation

		public Task<EntityType> FetchAsync(string id)
		{
			return FetchAsync(id, CancellationToken.None);
		}

		public Task<EntityType> FetchAsync(string id, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task<System.Collections.Generic.List<EntityType>> FetchAsync(int offset, int count)
		{
			return FetchAsync(offset, count, CancellationToken.None);
		}

		public Task<System.Collections.Generic.List<EntityType>> FetchAsync(int offset, int count, CancellationToken token)
		{
			throw new NotImplementedException();
		}

		#endregion
		
	}
}


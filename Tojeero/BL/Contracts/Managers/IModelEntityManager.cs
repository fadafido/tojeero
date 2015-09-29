using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public interface IModelEntityManager<EntityType> where EntityType : IModelEntity
	{
		Task<EntityType> FetchAsync(string id);
		Task<EntityType> FetchAsync(string id, CancellationToken token);

		Task<List<EntityType>> FetchAsync(int offset, int count);
		Task<List<EntityType>> FetchAsync(int offset, int count, CancellationToken token);
	}
}


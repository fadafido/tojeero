using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IModelEntityManager<EntityType> where EntityType : IModelEntity
	{
		Task<EntityType> FetchAsync(string id);
		Task<List<EntityType>> Fetch(int offset, int count);
	}
}


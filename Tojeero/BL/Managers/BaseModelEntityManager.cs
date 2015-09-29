using System;

namespace Tojeero.Core
{
	public class BaseModelEntityManager<EntityType> : IModelEntityManager<EntityType>
		where EntityType : IModelEntity
	{
		#region IModelEntityManager implementation

		public System.Threading.Tasks.Task<EntityType> FetchAsync(string id)
		{
			throw new NotImplementedException();
		}
		public System.Threading.Tasks.Task<System.Collections.Generic.List<EntityType>> Fetch(int offset, int count)
		{
			throw new NotImplementedException();
		}

		#endregion
		
	}
}


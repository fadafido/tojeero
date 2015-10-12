using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IQueryLoader<T>
	{
		string ID { get; }

		Task<IEnumerable<T>> LocalQuery();

		Task<IEnumerable<T>> RemoteQuery();
	}

	public interface IModelEntityManager
	{
		IRestRepository Rest { get; }
		ICacheRepository Cache { get; }
		Task<IEnumerable<T>> Fetch<T, Entity>(IQueryLoader<T> loader, double? expiresIn = null);
	}
}


using System;

namespace Tojeero.Core
{
	public interface IModelEntityManager : IRepository
	{
		IRestRepository Rest { get; }
		ICacheRepository Cache { get; }
	}
}


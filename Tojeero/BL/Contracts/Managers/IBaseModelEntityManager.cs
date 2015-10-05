using System;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IBaseModelEntityManager
	{
		Task ClearCache();
	}
}


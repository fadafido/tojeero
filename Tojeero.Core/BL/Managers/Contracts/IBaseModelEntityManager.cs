using System.Threading.Tasks;

namespace Tojeero.Core.Managers.Contracts
{
	public interface IBaseModelEntityManager
	{
		Task ClearCache();
	}
}


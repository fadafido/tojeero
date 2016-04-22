using PCLStorage;

namespace Tojeero.Core.Services.Contracts
{
	public interface IDeviceContextService
	{
		IFolder CacheFolder { get; }
	}
}


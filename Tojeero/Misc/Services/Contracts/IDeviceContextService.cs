using System;
using PCLStorage;

namespace Tojeero.Core.Services
{
	public interface IDeviceContextService
	{
		IFolder CacheFolder { get; }
	}
}


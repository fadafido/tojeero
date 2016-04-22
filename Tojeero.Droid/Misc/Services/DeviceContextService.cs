using PCLStorage;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Droid.Services
{
	public class DeviceContextService : IDeviceContextService
	{
		#region IDeviceContextService implementation

		public PCLStorage.IFolder CacheFolder
		{
			get
			{
				return FileSystem.Current.LocalStorage.GetFolderAsync("../cache").Result;
			}
		}

		#endregion
	}
}


using System;
using Tojeero.Core.Services;
using PCLStorage;


namespace Tojeero.iOS
{
	public class DeviceContextService : IDeviceContextService
	{
		#region IDeviceContextService implementation

		public PCLStorage.IFolder CacheFolder
		{
			get
			{
				return FileSystem.Current.LocalStorage.GetFolderAsync("Caches").Result;
			}
		}

		#endregion
	}
}


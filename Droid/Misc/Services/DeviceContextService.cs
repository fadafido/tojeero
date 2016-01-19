using System;
using Tojeero.Core.Services;
using PCLStorage;


namespace Tojeero.Droid
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


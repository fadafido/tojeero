using PCLStorage;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.iOS.Services
{
    public class DeviceContextService : IDeviceContextService
    {
        #region IDeviceContextService implementation

        public IFolder CacheFolder
        {
            get { return FileSystem.Current.LocalStorage.GetFolderAsync("Caches").Result; }
        }

        #endregion
    }
}
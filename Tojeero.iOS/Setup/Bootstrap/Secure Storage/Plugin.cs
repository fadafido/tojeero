// Plugin.cs
// (c) Copyright Christian Ruiz @_christian_ruiz
// MvvmCross - Secure Storage Plugin is licensed using Microsoft Public License (Ms-PL)
// 

using Beezy.MvvmCross.Plugins.SecureStorage;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;

namespace Tojeero.iOS.Bootstrap.Secure_Storage
{
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.RegisterSingleton<IMvxProtectedData>(new MvxTouchProtectedData());
        }
    }
}

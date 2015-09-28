using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Connectivity.Plugin;
using Connectivity.Plugin.Abstractions;
using Cirrious.CrossCore;

namespace Tojeero.Core.ViewModels
{
	public class LoadableNetworkViewModel : LoadableViewModel
	{
		#region Constructors

		public LoadableNetworkViewModel()
			: base()
		{
			var connectivity = CrossConnectivity.Current;
			connectivity.ConnectivityChanged+= (object sender, ConnectivityChangedEventArgs e) => {
				Mvx.Trace("CONNECTIVITY CHANGED TO - {0}", e.IsConnected ? "CONNECTED to " + string.Join(", ", connectivity.ConnectionTypes) : "DISCONNECTED");
			};
		}

		#endregion
	}
}


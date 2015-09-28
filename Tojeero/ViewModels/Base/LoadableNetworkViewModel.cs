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
				IsNetworkAvailable = e.IsConnected;
			};
			IsNetworkAvailable = connectivity.IsConnected;
		}

		#endregion

		#region Properties

		private bool _isNetworkAvailable;
		public bool IsNetworkAvailable
		{ 
			get
			{
				return _isNetworkAvailable; 
			}
			set
			{
				if (_isNetworkAvailable != value)
				{
					_isNetworkAvailable = value; 
					RaisePropertyChanged(() => IsNetworkAvailable); 
					RaisePropertyChanged(() => NoNetworkWarning); 
				}

			}
		}

		private static string _noNetworkLabel = "No network connection available.";
		public string NoNetworkWarning
		{ 
			get
			{
				return IsNetworkAvailable ? null : _noNetworkLabel; 
			}
		}
		#endregion
	}
}


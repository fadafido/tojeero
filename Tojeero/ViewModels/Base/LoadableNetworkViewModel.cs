using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Connectivity.Plugin;
using Connectivity.Plugin.Abstractions;
using Cirrious.CrossCore;
using Tojeero.Core.Resources;

namespace Tojeero.Core.ViewModels
{
	public class LoadableNetworkViewModel : LoadableViewModel
	{
		#region Constructors

		public LoadableNetworkViewModel()
			: base()
		{
			var connectivity = CrossConnectivity.Current;
			connectivity.ConnectivityChanged += handleNetworkConnectionChanged;
			IsNetworkAvailable = connectivity.IsConnected;
		}

		#endregion

		#region Properties

		public static string IsNetworkAvailablePropertyName = "IsNetworkAvailable";
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

		private static string _noNetworkLabel = AppResources.MessageNoInternetWarning;
		public virtual string NoNetworkWarning
		{ 
			get
			{
				return IsNetworkAvailable ? null : _noNetworkLabel; 
			}
		}
		#endregion

		#region Protected API

		protected virtual void handleNetworkConnectionChanged (object sender, ConnectivityChangedEventArgs e)
		{
			IsNetworkAvailable = e.IsConnected;
		}

		#endregion
	}
}


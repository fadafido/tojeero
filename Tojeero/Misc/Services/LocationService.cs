using System;
using Cirrious.MvvmCross.Plugins.Location;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;

namespace Tojeero.Core.Services
{
	public class LocationService : ILocationService
	{
		#region Private Fields and Properties

		private readonly IMvxLocationWatcher _locationWatcher;
		private readonly IMvxMessenger _messenger;

		#endregion

		#region Constructors

		public LocationService(IMvxLocationWatcher locationWatcher, IMvxMessenger messenger)
		{
			_locationWatcher = locationWatcher;
			_messenger = messenger;
			start();
		}

		#endregion

		#region ILocationService implementation

		MvxGeoLocation _lastKnownLocation;
		public MvxGeoLocation LastKnownLocation
		{
			get
			{
				return _lastKnownLocation;
			}
			private set
			{ 
				_lastKnownLocation = value;
				_messenger.Publish(new LocationUpdatedMessege(this, _lastKnownLocation));
			}
		}
			
		#endregion

		#region Location Watcher

		void OnLocation(MvxGeoLocation location)
		{
			this.LastKnownLocation = location;
		}


		void OnError(MvxLocationError error)
		{
			Mvx.Error("Failed location retrieval. {0}", error);
		}

		#endregion

		#region Utility Methods

		private void start()
		{
			_locationWatcher.Start(new MvxLocationOptions(),
				(loc) =>
				{
					OnLocation(loc);
				}, 
				(err) =>
				{
					OnError(err);
				});
		}

		#endregion
	}
}


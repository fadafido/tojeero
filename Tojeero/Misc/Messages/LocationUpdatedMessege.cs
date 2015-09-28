using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.Plugins.Location;

namespace Tojeero.Core
{
	public class LocationUpdatedMessege : MvxMessage
	{
		#region Constructors

		public LocationUpdatedMessege(object sender, MvxGeoLocation location = null)
			: base(sender)
		{
			Location = location;
		}

		#endregion

		#region Properties

		public MvxGeoLocation Location { get; set; }

		#endregion
	}
}


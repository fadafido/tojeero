using Cirrious.MvvmCross.Plugins.Location;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Core.Messages
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
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Messages
{
    public class StoreFilterChangedMessage : MvxMessage
    {
        #region Constructors

        public StoreFilterChangedMessage(object sender, IStoreFilter productFilter)
            : base(sender)
        {
            StoreFilter = productFilter;
        }

        #endregion

        #region Properties

        public IStoreFilter StoreFilter { get; set; }

        #endregion
    }
}
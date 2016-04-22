using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Messages
{
    public class ProductChangedMessage : MvxMessage
    {
        public ProductChangedMessage(object sender, IProduct product, EntityChangeType changeType)
            : base(sender)
        {
            Product = product;
            ChangeType = changeType;
        }

        public IProduct Product { get; set; }
        public EntityChangeType ChangeType { get; set; }
    }
}
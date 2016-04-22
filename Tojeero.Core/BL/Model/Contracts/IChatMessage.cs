using System;

namespace Tojeero.Core.Model.Contracts
{
    public interface IChatMessage
    {
        string ID { get; set; }
        string Text { get; set; }
        string SenderID { get; set; }
        string RecipientID { get; set; }
        DateTimeOffset? SentDate { get; set; }
        DateTimeOffset? DeliveryDate { get; set; }
        string ChannelID { get; set; }
        string ProductID { get; set; }
    }
}

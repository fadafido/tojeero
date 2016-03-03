using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tojeero.Core
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

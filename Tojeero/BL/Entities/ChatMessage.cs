using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
    public class ChatMessage : IChatMessage
    {
        public string ID { get; set; }
        public string Text { get; set; }
        public string SenderID { get; set; }
        public string RecipientID { get; set; }
        public DateTimeOffset? SentDate { get; set; }
        public DateTimeOffset? DeliveryDate { get; set; }
        public string ChannelID { get; set; }
        public string ProductID { get; set; }
    }
}

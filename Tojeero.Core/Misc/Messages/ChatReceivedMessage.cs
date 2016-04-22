using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Messages
{
    public class ChatReceivedMessage: MvxMessage
    {
        public ChatReceivedMessage(object sender, IChatMessage message, string channelID)
            :base(sender)
        {
            Message = message;
            ChannelID = channelID;
        }

        public IChatMessage Message { get; set; }
        public string ChannelID { get; set; }
    }
}

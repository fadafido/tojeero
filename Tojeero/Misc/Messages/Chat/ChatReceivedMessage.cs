using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Core.Messages
{
    public class ChatReceivedMessage: MvxMessage
    {
        public ChatReceivedMessage(object sender, IChatResponseMessage message)
            :base(sender)
        {
            Message = message;
        }

        public IChatResponseMessage Message { get; set; }
    }
}

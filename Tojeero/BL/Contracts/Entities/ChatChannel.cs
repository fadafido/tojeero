using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Forms.BL.Contracts;

namespace Tojeero.Core
{
    public class ChatChannel : IChatChannel
    {
        public string ChannelID { get; set; }
        public string SenderID { get; set; }
        public string SenderProfilePictureUrl { get; set; }
        public string RecipientID { get; set; }
        public string RecipientProfilePictureUrl { get; set; }
    }
}

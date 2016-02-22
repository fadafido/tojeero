using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tojeero.Core
{
    public interface IChatChannel
    {
        string ChannelID { get; set; }
        string SenderID { get; set; }
        string SenderProfilePictureUrl { get; set; }
        string RecipientID { get; set; }
        string RecipientProfilePictureUrl { get; set; }
    }
}

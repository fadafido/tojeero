using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
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

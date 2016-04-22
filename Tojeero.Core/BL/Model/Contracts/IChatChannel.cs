namespace Tojeero.Core.Model.Contracts
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
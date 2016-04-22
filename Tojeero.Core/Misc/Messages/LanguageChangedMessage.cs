using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Model;

namespace Tojeero.Core.Messages
{
    public class LanguageChangedMessage : MvxMessage
    {
        #region Constructors

        public LanguageChangedMessage(object sender, LanguageCode language)
            : base(sender)
        {
            Language = language;
        }

        #endregion

        #region Properties

        public LanguageCode Language { get; set; }

        #endregion
    }
}
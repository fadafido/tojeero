using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core;
using Xamarin.Forms;

namespace Tojeero.Forms.ViewModels.Misc
{
    public class ChatMessageViewModel : MvxViewModel
    {
        #region Constructors

        public ChatMessageViewModel(IChatMessage message = null, string profilePictureUrl = null, bool isSentByCurrentUser = true)
        {
            _message = message;
            _isSentByCurrentUser = isSentByCurrentUser;
            ProfilePictureUrl = profilePictureUrl;
            updateViewModel();
        }

        #endregion

        #region Properties

        private IChatMessage _message;
        public IChatMessage Message
        {
            get
            {
                return _message;
            }
            private set
            {
                if (_message != value)
                {
                    _message = value;
                    updateViewModel();
                }
            }
        }

        private bool _isSentByCurrentUser;
        public bool IsSentByCurrentUser
        {
            get
            {
                return _isSentByCurrentUser;
            }
            private set
            {
                if(_isSentByCurrentUser != value)
                {
                    _isSentByCurrentUser = value;
                    updateViewModel();
                }
            }
        }

        public string ProfilePictureUrl { get; private set; }
        public string FormattedDeliveryDate { get; private set; }

        #endregion

        #region Utility methods

        private void updateViewModel()
        {
            FormattedDeliveryDate = $"{Message?.DeliveryDate:ddd, d MMM HH:mm}";
            RaiseAllPropertiesChanged();
            
        }

        #endregion
    }
}

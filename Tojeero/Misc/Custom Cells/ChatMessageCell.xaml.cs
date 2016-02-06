using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Forms.ViewModels.Misc;
using Xamarin.Forms;

namespace Tojeero.Forms
{
    public partial class ChatMessageCell : ViewCell
    {
        public ChatMessageCell()
        {
            InitializeComponent();
        }

        public ChatMessageViewModel ViewModel => this.BindingContext as ChatMessageViewModel;

        public Label MessageLabel => ViewModel?.IsSentByCurrentUser == true ? sentMessageLabel : receivedMessageLabel;

        public Size TotallPadding
        {
            get
            {
                //Horizontal padding = {cell left/right padding (40 + 10)} + {message label left/right padding (10 + 10)};
                var horizontalPadding = 40 + 10 + 10 + 10;
                //Verticall padding = {date Label Height(25)} + {message label top/bottom padding (5 + 5)};
                var verticalPadding = 25 + 5 + 5;
                return new Size(horizontalPadding, verticalPadding);
            }
        }
    }
}

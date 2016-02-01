using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.ViewModels.Chat;
using Xamarin.Forms;

namespace Tojeero.Forms.Pages.Chat
{
    public partial class ChatPage : ContentPage
    {
        #region Constructors
        public ChatPage()
        {
            this.ViewModel = MvxToolbox.LoadViewModel<ChatChannelViewModel<ChatMessage>>();
            this.ViewModel.ChannelID = "test_channel";
            InitializeComponent();
        }
        #endregion

        #region Properties
        private ChatChannelViewModel<ChatMessage> _viewModel;
        public ChatChannelViewModel<ChatMessage> ViewModel  
        {
            get
            {
                return _viewModel;
            }
            set
            {
                if(_viewModel != value)
                {
                    _viewModel = value;
                    this.BindingContext = value;
                }
            }
        }
        #endregion
    }
}

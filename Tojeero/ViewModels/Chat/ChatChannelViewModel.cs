using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core;
using Tojeero.Core.Messages;
using Tojeero.Core.Services;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.BL.Contracts;
using Tojeero.Forms.BL.Entities;
using Tojeero.Forms.ViewModels.Misc;

namespace Tojeero.Forms.ViewModels.Chat
{
    public class ChatChannelViewModel : BaseUserViewModel
    {
        #region Private fields and properties
        private readonly MvxSubscriptionToken _chatMessageReceivedSubscribtionToken;
        private readonly IChatService _chatService;
        private readonly IProductManager _productManager;
        #endregion

        #region Constructors

        public ChatChannelViewModel(
            IChatService chatService, 
            IAuthenticationService authService, 
            IMvxMessenger messenger,
            IProductManager productManager) : base(authService, messenger)
        {
            _chatService = chatService;
            _productManager = productManager;
            _chatMessageReceivedSubscribtionToken = _messenger.Subscribe<ChatReceivedMessage>(handleChatMessageReceived);
            _messages = new ObservableCollection<ChatMessageViewModel>();
        }

        #endregion

        #region Properties

        public Action<ChatMessageViewModel> ScrollToMessageAction { get; set; }

        private ObservableCollection<ChatMessageViewModel> _messages;
        public ObservableCollection<ChatMessageViewModel> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }

        private IChatChannel _channel;
        public IChatChannel Channel
        { 
            get  
            {
                return _channel; 
            }
            set 
            {
                _channel = value; 
                RaisePropertyChanged(() => Channel); 
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
            }
        }  

        private string _currentMessage;
        public string CurrentMessage
        {
            get
            {
                return _currentMessage;
            }
            set
            {
                _currentMessage = value;
                RaisePropertyChanged(() => CurrentMessage);
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
            }
        }

        private bool _isSendingMessage;
        public bool IsSendingMessage
        { 
            get  
            {
                return _isSendingMessage; 
            }
            set 
            {
                _isSendingMessage = value; 
                RaisePropertyChanged(() => IsSendingMessage);
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
            }
        }  

        #endregion

        #region Commands

        private MvxCommand _sendMessageCommand;

        public ICommand SendMessageCommand
        {
            get
            {
                _sendMessageCommand = _sendMessageCommand ?? new MvxCommand(async () =>
                {
                    if (CanExecuteSendMessageCommand)
                    {
                        await sendMessage();
                    }
                }, () => CanExecuteSendMessageCommand);
                return _sendMessageCommand;
            }
        }

        public bool CanExecuteSendMessageCommand
        {
            get { return 
                    !string.IsNullOrEmpty(CurrentMessage) && 
                    !IsSendingMessage && 
                    this.Channel != null &&
                    !string.IsNullOrWhiteSpace(Channel?.ChannelID) &&
                    !string.IsNullOrWhiteSpace(Channel?.SenderID) &&
                    !string.IsNullOrWhiteSpace(Channel?.RecipientID);
            }
        }

        #endregion

        #region Utility methods

        private void handleChatMessageReceived(ChatReceivedMessage message)
        {
            var receivedMessage = message.Message.GetContent<ChatMessage>();
            if (receivedMessage == null)
                return;
            receivedMessage.DeliveryDate = DateTimeOffset.Now;
            receivedMessage.ProductID = "X8WiR8PqfO";
            var isSentByCurrentUser = receivedMessage.SenderID == Channel?.SenderID;
            var profilePictureUrl = isSentByCurrentUser
                ? Channel?.SenderProfilePictureUrl
                : Channel?.RecipientProfilePictureUrl;
            var chatMessage = new ChatMessageViewModel(_productManager, receivedMessage, profilePictureUrl, isSentByCurrentUser);
            _messages.Add(chatMessage);
            ScrollToMessageAction.Fire(chatMessage);
        }

        private async Task sendMessage()
        {
            this.IsSendingMessage = true;
            try
            {
                await _chatService.SubscribeToChannelAsync(Channel.ChannelID);
                var message = new ChatMessage()
                {
                    Text = CurrentMessage,
                    SenderID = Channel?.SenderID,
                    RecipientID = Channel?.RecipientID
                };
                await _chatService.SendMessageAsync(message, Channel.ChannelID);
                this.CurrentMessage = null;
            }
            catch (OperationCanceledException ex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                this.IsSendingMessage = false;
            }
        }

        #endregion

    }
}

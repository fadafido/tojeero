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
using Tojeero.Core.ViewModels;
using Tojeero.Forms.BL.Entities;

namespace Tojeero.Forms.ViewModels.Chat
{
    public class ChatChannelViewModel<T> : BaseUserViewModel
        where T : IChatMessage, new()
    {
        #region Private fields and properties
        private readonly MvxSubscriptionToken _chatMessageReceivedSubscribtionToken;
        private readonly IChatService _chatService;
        #endregion

        #region Constructors

        public ChatChannelViewModel(IChatService chatService, IAuthenticationService authService, IMvxMessenger messenger) : base(authService, messenger)
        {
            _chatService = chatService;
            _chatMessageReceivedSubscribtionToken = _messenger.Subscribe<ChatReceivedMessage>(handleChatMessageReceived);
            _messages = new ObservableCollection<T>();
        }

        #endregion

        #region Properties

        private ObservableCollection<T> _messages;
        public ObservableCollection<T> Messages
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

        private string _channelID;
        public string ChannelID
        { 
            get  
            {
                return _channelID; 
            }
            set 
            {
                _channelID = value; 
                RaisePropertyChanged(() => ChannelID); 
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
            get { return !string.IsNullOrEmpty(CurrentMessage) && !IsSendingMessage && !string.IsNullOrWhiteSpace(ChannelID); }
        }

        #endregion

        #region Utility methods

        private void handleChatMessageReceived(ChatReceivedMessage message)
        {
            _messages.Add(message.Message.GetContent<T>());
        }

        private async Task sendMessage()
        {
            this.IsSendingMessage = true;
            try
            {
                await _chatService.SubscribeToChannelAsync(this.ChannelID);
                await _chatService.SendMessageAsync(new T() {Text = CurrentMessage}, ChannelID);
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Tojeero.Forms.Resources;
using Tojeero.Forms.ViewModels.Misc;

namespace Tojeero.Forms.ViewModels.Chat
{
    public class ChatViewModel : BaseUserViewModel
    {
        #region Private fields and properties
        private readonly MvxSubscriptionToken _chatMessageReceivedSubscribtionToken;
        private readonly IChatService _chatService;
        private readonly IProductManager _productManager;
        private DateTimeOffset? _lastMessageDate;
        private int _pageSize = 3;

        private bool _allMessagesLoaded;
        public static string AllMessagesLoadedProperty = "AllMessagesLoaded";
        private bool AllMessagesLoaded
        { 
            get  
            {
                return _allMessagesLoaded; 
            }
            set 
            {
                _allMessagesLoaded = value; 
                RaisePropertyChanged(() => AllMessagesLoaded);
            }
        }  
        #endregion

        #region Constructors

        public ChatViewModel(
            IChatService chatService, 
            IAuthenticationService authService, 
            IMvxMessenger messenger,
            IProductManager productManager) : base(authService, messenger)
        {
            _chatService = chatService;
            _productManager = productManager;
            _chatMessageReceivedSubscribtionToken = _messenger.Subscribe<ChatReceivedMessage>(handleChatMessageReceived);
            _messages = new ObservableCollection<ChatMessageViewModel>();
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Properties

        public Action<ChatMessageViewModel> ScrollToMessageAction { get; set; }

        public event EventHandler WillChangeMessagesCollection;
        public event EventHandler DidChangeMessagesCollection;

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
        public static string ChannelProperty = "Channel";
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
            }
        }  

        private ProductViewModel _productViewModel;
        public static string ProductViewModelProperty = "ProductViewModel";
        public ProductViewModel ProductViewModel
        { 
            get  
            {
                return _productViewModel ?? new ProductViewModel(); 
            }
            set 
            {
                _productViewModel = value; 
                RaisePropertyChanged(() => ProductViewModel); 
            }
        }  

        private string _currentMessage;
        public static string CurrentMessageProperty = "CurrentMessage";
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
            }
        }

        private bool _isSendingMessage;
        public static string IsSendingMessageProperty = "IsSendingMessage";
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
            }
        }

        private bool _isSubscribed;
        public static string IsSubscribedProperty = "IsSubscribed";
        public bool IsSubscribed
        {
            get
            {
                return _isSubscribed;
            }
            private set
            {
                _isSubscribed = value;
                RaisePropertyChanged(() => IsSubscribed);
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

        public bool CanExecuteSendMessageCommand => !string.IsNullOrEmpty(CurrentMessage) &&
                                                    IsSubscribed &&
                                                    !IsSendingMessage &&
                                                    CanExecuteInitCommand;

        private MvxCommand _initCommmand;
        public ICommand InitCommand
        {
            get
            {
                _initCommmand = _initCommmand ?? new MvxCommand(init);
                return _initCommmand;
            }
        }

        public bool CanExecuteInitCommand => this.Channel != null &&
                                             !string.IsNullOrWhiteSpace(Channel?.ChannelID) &&
                                             !string.IsNullOrWhiteSpace(Channel?.SenderID) &&
                                             !string.IsNullOrWhiteSpace(Channel?.RecipientID);

        private MvxCommand _loadMoreMessagesCommand;

        public ICommand LoadMoreMessagesCommand
        {
            get
            {
                _loadMoreMessagesCommand = _loadMoreMessagesCommand ?? new MvxCommand(loadMoreMessages);
                return _loadMoreMessagesCommand;
            }
        }

        public bool CanLoadMoreMessages => CanExecuteInitCommand && IsSubscribed && !IsLoading && !_allMessagesLoaded;

        #endregion

        #region Utility methods

        private void handleChatMessageReceived(ChatReceivedMessage message)
        {
            var receivedMessage = message.Message;
            if (receivedMessage == null)
                return;
            var chatMessage = getChatMessageViewModel(receivedMessage);
            _messages.Add(chatMessage);
            ScrollToMessageAction.Fire(chatMessage);
        }

        private async Task sendMessage()
        {
            this.IsSendingMessage = true;
            try
            {
                var message = new ChatMessage()
                {
                    Text = CurrentMessage,
                    SenderID = Channel?.SenderID,
                    RecipientID = Channel?.RecipientID,
                    ProductID = ProductViewModel?.Product?.ID
                };
                await _chatService.SendMessageAsync(_authService.CurrentUser, message, Channel.ChannelID);
                CurrentMessage = null;
                ProductViewModel.Product = null;
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

        private async void init()
        {
            if (!CanExecuteInitCommand)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            string failure = "";
            try
            {
                //Subscribe to the channel
                await _chatService.SubscribeToChannelAsync(_authService.CurrentUser, Channel.ChannelID);
                IsSubscribed = true;

                //Load previous messages
                await loadNextPage();
            }
            catch (OperationCanceledException)
            {
                failure = AppResources.MessageLoadingTimeOut;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading chat history.", LoggingLevel.Error, true);
                failure = AppResources.MessageLoadingFailed;
            }
            finally
            {
                StopLoading(failure);
            }
        }

        private async void loadMoreMessages()
        {
            if (!CanLoadMoreMessages)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            string failure = "";
            try
            {
                //Load previous messages
                await loadNextPage();
            }
            catch (OperationCanceledException)
            {
                failure = AppResources.MessageLoadingTimeOut;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occurred while loading chat history.", LoggingLevel.Error, true);
                failure = AppResources.MessageLoadingFailed;
            }
            finally
            {
                StopLoading(failure);
            }
        }

        private async Task loadNextPage()
        {
            if (AllMessagesLoaded)
                return;

            var previousCount = Messages.Count;
            var messages = await _chatService.GetMessagesAsync(_authService.CurrentUser, Channel.ChannelID, _lastMessageDate, _pageSize);
            messages = messages.Reverse();
            WillChangeMessagesCollection.Fire(this, EventArgs.Empty);
            Messages.InsertSorted(messages.Select(getChatMessageViewModel), Comparers.ChatMessage);
            DidChangeMessagesCollection.Fire(this, EventArgs.Empty);
            if (messages.Any())
                _lastMessageDate = messages.Last().DeliveryDate;
            if (Messages.Count - previousCount < _pageSize)
            {
                AllMessagesLoaded = true;
            }
        }

        private ChatMessageViewModel getChatMessageViewModel(IChatMessage receivedMessage)
        {
            var isSentByCurrentUser = receivedMessage.SenderID == Channel?.SenderID;
            var profilePictureUrl = isSentByCurrentUser
                ? Channel?.SenderProfilePictureUrl
                : Channel?.RecipientProfilePictureUrl;
            var chatMessage = new ChatMessageViewModel(_productManager, receivedMessage, profilePictureUrl, isSentByCurrentUser);
            return chatMessage;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChannelProperty)
            {
                RaisePropertyChanged(() => CanExecuteInitCommand);
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
                RaisePropertyChanged(() => CanLoadMoreMessages);
            }
            else if (e.PropertyName == IsSendingMessageProperty)
            {
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
            }
            else if (e.PropertyName == CurrentMessageProperty)
            {
                RaisePropertyChanged(() => CanExecuteSendMessageCommand);
            }
            else if (e.PropertyName == IsLoadingProperty)
            {
                RaisePropertyChanged(() => CanExecuteInitCommand);
                RaisePropertyChanged(() => CanLoadMoreMessages);
            }
            else if (e.PropertyName == AllMessagesLoadedProperty)
            {
                RaisePropertyChanged(() => CanLoadMoreMessages);
            }
        }

        #endregion

    }
}

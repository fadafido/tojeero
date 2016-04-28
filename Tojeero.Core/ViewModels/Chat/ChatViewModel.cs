﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;
using Tojeero.Core.ViewModels.Product;

namespace Tojeero.Core.ViewModels.Chat
{
    public class ChatViewModel : BaseUserViewModel
    {
        #region Private fields and properties

        private readonly MvxSubscriptionToken _chatMessageReceivedSubscribtionToken;
        private readonly IChatService _chatService;
        private readonly IProductManager _productManager;
        private DateTimeOffset? _lastMessageDate;
        private readonly int _pageSize = 3;

        private bool _allMessagesLoaded;
        public static string AllMessagesLoadedProperty = "AllMessagesLoaded";

        private bool AllMessagesLoaded
        {
            get { return _allMessagesLoaded; }
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
            _chatMessageReceivedSubscribtionToken = _messenger.Subscribe<ChatReceivedMessage>(HandleChatMessageReceived);
            _messages = new ObservableCollection<ChatMessageViewModel>();
            PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Lifecycle management

        public override void OnAppearing()
        {
            base.OnAppearing();
            InitCommand.Execute(null);
        }

        #endregion

        #region Properties

        public Action<ChatMessageViewModel> ScrollToMessageAction { get; set; }
        public Action<IProduct> ShowProductDetailsAction { get; set; }

        public event EventHandler WillChangeMessagesCollection;
        public event EventHandler DidChangeMessagesCollection;

        private ObservableCollection<ChatMessageViewModel> _messages;

        public ObservableCollection<ChatMessageViewModel> Messages
        {
            get { return _messages; }
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
            get { return _channel; }
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
            get { return _productViewModel ?? new ProductViewModel(); }
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
            get { return _currentMessage; }
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
            get { return _isSendingMessage; }
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
            get { return _isSubscribed; }
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
                        await SendMessage();
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
                _initCommmand = _initCommmand ?? new MvxCommand(Init, () => CanExecuteInitCommand);
                return _initCommmand;
            }
        }

        public bool CanExecuteInitCommand => Channel != null &&
                                             !string.IsNullOrWhiteSpace(Channel?.ChannelID) &&
                                             !string.IsNullOrWhiteSpace(Channel?.SenderID) &&
                                             !string.IsNullOrWhiteSpace(Channel?.RecipientID);

        private MvxCommand _loadMoreMessagesCommand;

        public ICommand LoadMoreMessagesCommand
        {
            get
            {
                _loadMoreMessagesCommand = _loadMoreMessagesCommand ?? new MvxCommand(LoadMoreMessages);
                return _loadMoreMessagesCommand;
            }
        }

        public bool CanLoadMoreMessages => CanExecuteInitCommand && IsSubscribed && !IsLoading && !_allMessagesLoaded;

        private MvxCommand<ChatMessageViewModel> _messageSelectedCommand;
        public ICommand MessageSelectedCommand
        {
            get
            {
                _messageSelectedCommand = _messageSelectedCommand ?? new MvxCommand<ChatMessageViewModel>(m =>
                {
                    if (m.Product?.Product != null)
                    {
                        ShowProductDetailsAction?.Invoke(m.Product.Product);
                    }
                });
                return _messageSelectedCommand;
            }
        }


        #endregion

        #region Utility methods

        private void HandleChatMessageReceived(ChatReceivedMessage message)
        {
            var receivedMessage = message.Message;
            if (receivedMessage == null)
                return;
            var chatMessage = GetChatMessageViewModel(receivedMessage);
            _messages.Add(chatMessage);
            ScrollToMessageAction.Fire(chatMessage);
        }

        private async Task SendMessage()
        {
            IsSendingMessage = true;
            string failure = null;
            try
            {
                var message = new ChatMessage
                {
                    ID = Guid.NewGuid().ToString(),
                    Text = CurrentMessage,
                    SenderID = Channel.SenderID,
                    RecipientID = Channel.RecipientID,
                    SentDate = DateTimeOffset.Now,
                    ProductID = ProductViewModel?.Product?.ID,
                    ChannelID = Channel.ChannelID
                };
                await _chatService.SendMessageAsync(_authService.CurrentUser, message, Channel.ChannelID);
                CurrentMessage = null;
                ProductViewModel.Product = null;
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log("Sending message timeout");
                failure = AppResources.MessageChatMessageSendingTimeout;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Message sending failed", LoggingLevel.Error, true);
                failure = AppResources.MessageChatMessageSendingFailure;
            }
            finally
            {
                IsSendingMessage = false;
                if (!string.IsNullOrEmpty(failure))
                {
                }
            }
        }

        private async void Init()
        {
            if (!CanExecuteInitCommand)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            var failure = "";
            try
            {
                //Subscribe to the channel
                await _chatService.SubscribeToChannelAsync(_authService.CurrentUser, Channel.ChannelID);
                IsSubscribed = true;

                //Load previous messages
                await LoadNextPage();
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

        private async void LoadMoreMessages()
        {
            if (!CanLoadMoreMessages)
                return;
            StartLoading(AppResources.MessageGeneralLoading);
            var failure = "";
            try
            {
                //Load previous messages
                await LoadNextPage();
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

        private async Task LoadNextPage()
        {
            if (AllMessagesLoaded)
                return;

            var previousCount = Messages.Count;
            var messages =
                await
                    _chatService.GetMessagesAsync(_authService.CurrentUser, Channel.ChannelID, _lastMessageDate,
                        _pageSize);
            messages = messages.Reverse();
            WillChangeMessagesCollection.Fire(this, EventArgs.Empty);
            Messages.InsertSorted(messages.Select(GetChatMessageViewModel), Comparers.ChatMessage);
            DidChangeMessagesCollection.Fire(this, EventArgs.Empty);
            if (messages.Any())
                _lastMessageDate = messages.Last().DeliveryDate;
            if (Messages.Count - previousCount < _pageSize)
            {
                AllMessagesLoaded = true;
            }
        }

        private ChatMessageViewModel GetChatMessageViewModel(IChatMessage receivedMessage)
        {
            var isSentByCurrentUser = receivedMessage.SenderID == Channel?.SenderID;
            var profilePictureUrl = isSentByCurrentUser
                ? Channel?.SenderProfilePictureUrl
                : Channel?.RecipientProfilePictureUrl;
            var chatMessage = new ChatMessageViewModel(_productManager, receivedMessage, profilePictureUrl,
                isSentByCurrentUser);
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
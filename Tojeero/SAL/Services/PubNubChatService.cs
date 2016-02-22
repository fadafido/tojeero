using System;
using PubNubMessaging.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nito.AsyncEx;
using System.Threading;
using Cirrious.MvvmCross.Plugins.Messenger;
using Newtonsoft.Json;
using Tojeero.Core.Messages;
using Tojeero.Core.Toolbox;
using System.Linq;
using XLabs;

namespace Tojeero.Core.Services
{
	public class PubNubChatService : IChatService
	{
        #region Private fields and properties
        private readonly IMvxMessenger _messenger;
        private readonly Pubnub _pubnub;
	    private Dictionary<string, bool> _subscribtions;
        private object _subscribtionsLocker; 
        private Dictionary<string, AsyncReaderWriterLock> _channelLockers;
	    #endregion

        #region Constructors
        public PubNubChatService(IMvxMessenger messenger)
        {
            _messenger = messenger;
            _pubnub = new Pubnub(Constants.PubNubPublishKey, Constants.PubNubSubscribeKey);
            _subscribtions = new Dictionary<string, bool>();
            _channelLockers = new Dictionary<string, AsyncReaderWriterLock>();
            _subscribtionsLocker = new object();
        }
        #endregion
       
        #region IChatService implementation

        public async Task SubscribeToChannelAsync(string channelID)
        {
            await SubscribeToChannelAsync(channelID, CancellationToken.None);
        }

        public async Task SubscribeToChannelAsync(string channelID, CancellationToken token)
        {
            using (var writerLock = await getChannelLock(channelID).WriterLockAsync(token))
            {
                await subscribeToChannel(channelID);
            }
        }

        public async Task UnsubscribeFromChannelAsync(string channelID)
        {
            await UnsubscribeFromChannelAsync(channelID, CancellationToken.None);
        }

        public async Task UnsubscribeFromChannelAsync(string channelID, CancellationToken token)
        {
            using (var writerLock = await getChannelLock(channelID).WriterLockAsync(token))
            {
                await unsubscribeFromChannel(channelID);
            }
        }

        public async Task SendMessageAsync(IChatMessage message, string channelID)
        {
            await SendMessageAsync(message, channelID, CancellationToken.None);
        }

        public async Task SendMessageAsync(IChatMessage message, string channelID, CancellationToken token)
        {
            using (var writerLock = await getChannelLock(channelID).WriterLockAsync(token))
            {
                await sendMessage(message, channelID);
            }
        }

		public Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize)
        {
			return GetMessagesAsync(channelID, startDate, pageSize, CancellationToken.None);
        }

		public async Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize, CancellationToken token)
        {
			using (var readLock = await getChannelLock(channelID).ReaderLockAsync(token))
			{
				var result = await getMessages(channelID, startDate, pageSize);
				return result;
			}
        }	

	    public async Task<IEnumerable<IChatChannel>> FetchRecentChannelsAsync(string userID, int pageSize = -1, int offset = -1)
	    {
            throw new NotImplementedException();
        }

	    #endregion

        #region Utility methods

        private Task subscribeToChannel(string channelID)
        {
            var task = new TaskCompletionSource<bool>();
            if (_subscribtions.ContainsKey(channelID))
            {
                task.TrySetResult(true);
            }
            else
            {
                _pubnub.Subscribe<string>(channelID, result =>
                {
                    didReceiveMessage(channelID, result);
                }, s =>
                {
                    task.TrySetResult(true);
                    _subscribtions[channelID] = true;
                }, error =>
                {
                    task.TrySetException(new Exception(error.Description));
                });
            }
            return task.Task;
        }


	    private Task unsubscribeFromChannel(string channelID)
        {
            lock (_subscribtionsLocker)
            {
                if (_subscribtions.ContainsKey(channelID))
                    _subscribtions.Remove(channelID);
            }
            var task = new TaskCompletionSource<bool>();
            _pubnub.Unsubscribe(channelID, callback =>
            {
                task.TrySetResult(true);
            },
            null, null,   error =>
            {
                task.TrySetException(new Exception(error.Description));
            });
            return task.Task;
        }

        private Task sendMessage(IChatMessage message, string channelID)
        {
           var messageString = JsonConvert.SerializeObject(message);
           return sendMessage(messageString, channelID);
        }

        private Task sendMessage(string message, string channelID)
        {
            var task = new TaskCompletionSource<bool>();
            lock (_subscribtionsLocker)
            {
                bool isSubscribed;
                if (!(_subscribtions.TryGetValue(channelID, out isSubscribed) && isSubscribed))
                    task.TrySetException(new Exception("You should subscribe to a thread before trying to send a message."));
            }
            _pubnub.Publish(channelID, message, callback =>
            {
                task.TrySetResult(true);
            }, error =>
            {
                task.TrySetException(new Exception(error.Description));
            });
            return task.Task;
        }

		private Task<IEnumerable<IChatMessage>> getMessages(string channelID, DateTimeOffset? startDate, int pageSize)
        {
			var task = new TaskCompletionSource<IEnumerable<IChatMessage>>();

			if (startDate == null)
			{
				_pubnub.DetailedHistory<string>(channelID, pageSize, callback =>
					{
						var result = parseReceivedMessages(callback);
						task.TrySetResult(result);
					}, error =>
					{
						task.TrySetException(new Exception(error.Description));
					});
			}
			else
			{
				_pubnub.DetailedHistory<string>(channelID, startDate.Value.ToTimeToken(), -1, pageSize, false,
					callback =>
					{
						var result = parseReceivedMessages(callback);
						task.TrySetResult(result);
					}, error =>
					{
						task.TrySetException(new Exception(error.Description));
					});
			}

			return task.Task;
        }


	    private AsyncReaderWriterLock getChannelLock(string channelID)
	    {
	        lock (_subscribtionsLocker)
	        {
	            AsyncReaderWriterLock locker;
	            if (!_channelLockers.TryGetValue(channelID, out locker))
	            {
                    _channelLockers[channelID] = new AsyncReaderWriterLock();
	            }
	            return _channelLockers[channelID];
	        }
	    }

        private void didReceiveMessage(string channelID, string result)
        {
            IChatMessage message = null;
            DateTimeOffset messageDate = DateTimeOffset.Now;
            
            if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(result.Trim()))
            {
                List<object> deserializedMessage = _pubnub.JsonPluggableLibrary.DeserializeToListOfObject(result);
                if (deserializedMessage != null && deserializedMessage.Count > 0)
                {
                    object subscribedObject = (object)deserializedMessage[0];
                    long timeToken;
                    if (long.TryParse(deserializedMessage[1].ToString(), out timeToken))
                    {
						messageDate = timeToken.TimeTokenToDateTimeOffset();
                    }
                    if (subscribedObject != null)
                    {
                        message = JsonConvert.DeserializeObject<ChatMessage>(subscribedObject.ToString());
                        message.DeliveryDate = messageDate;
                    }
                }
            }
            _messenger.Publish(new ChatReceivedMessage(this, message, channelID));
        }

		private IEnumerable<IChatMessage> parseReceivedMessages(string result)
		{
			if (!string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(result.Trim()))
			{
				List<object> deserializedMessage = _pubnub.JsonPluggableLibrary.DeserializeToListOfObject(result);
				if (deserializedMessage != null && deserializedMessage.Count > 0)
				{
					object[] message = _pubnub.JsonPluggableLibrary.ConvertToObjectArray(deserializedMessage[0]);
                    long timeToken;
				    DateTimeOffset? messageDate = null;
                    if (long.TryParse(deserializedMessage[1].ToString(), out timeToken))
                    {
                        messageDate = timeToken.TimeTokenToDateTimeOffset();
                    }
                    if (message != null)
					{
						var messages = message.Select(m => JsonConvert.	DeserializeObject<ChatMessage>(m.ToString())).ToList();
					    foreach (var m in messages)
					    {
					        m.DeliveryDate = messageDate;
					    }
						return messages;
					}
				}
			}
			return new List<IChatMessage>();
		}
        #endregion

    }
}


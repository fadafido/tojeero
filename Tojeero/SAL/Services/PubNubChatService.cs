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
using Tojeero.Forms.BL.Contracts;
using Tojeero.Forms.BL.Entities;

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

        public async Task SendMessageAsync<T>(T message, string channelID)
        {
            await SendMessageAsync(message, channelID, CancellationToken.None);
        }

        public async Task SendMessageAsync<T>(T message, string channelID, CancellationToken token)
        {
            using (var writerLock = await getChannelLock(channelID).WriterLockAsync(token))
            {
                await sendMessage(message, channelID);
            }
        }

        public async Task SendMessageAsync(string message, string channelID)
        {
            await SendMessageAsync(message, channelID, CancellationToken.None);
        }

        public async Task SendMessageAsync(string message, string channelID, CancellationToken token)
        {
            using (var writerLock = await getChannelLock(channelID).WriterLockAsync(token))
            {
                await sendMessage(message, channelID);
            }
        }

        public Task<IEnumerable<T>> GetMessagesAsync<T>(string channelID, int pageSize, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetMessagesAsync<T>(string channelID, int pageSize, int offset, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetMessagesAsync(string channelID, int pageSize, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetMessagesAsync(string channelID, int pageSize, int offset, CancellationToken token)
        {
            throw new NotImplementedException();
        }

	    public async Task<List<IChatChannel>> FetchRecentChannelsAsync(string userID, int pageSize = -1, int offset = -1)
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
                    publishReceivedMessage(channelID, result);
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

        private Task sendMessage<T>(T message, string channelID)
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

        private Task<IEnumerable<T>> getMessages<T>(string channelID, int pageSize, int offset)
        {
            throw new NotImplementedException();
        }

        private Task<IEnumerable<string>> getMessages(string channelID, int pageSize, int offset)
        {
            throw new NotImplementedException();
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

        private void publishReceivedMessage(string channelID, string result)
        {
            string message = "";
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
                        message = subscribedObject.ToString();
                    }
                }
            }
            _messenger.Publish(new ChatReceivedMessage(this, new ChatResponseMessage(channelID, message, messageDate)));
        }
        #endregion

    }
}


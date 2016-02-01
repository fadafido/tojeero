using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IChatService
	{
	    Task SubscribeToChannelAsync(string channelID);
        Task SubscribeToChannelAsync(string channelID, CancellationToken token);
        Task UnsubscribeFromChannelAsync(string channelID);
        Task UnsubscribeFromChannelAsync(string channelID, CancellationToken token);
        Task SendMessageAsync<T>(T message, string channelID);
        Task SendMessageAsync<T>(T message, string channelID, CancellationToken token);
        Task SendMessageAsync(string message, string channelID);
        Task SendMessageAsync(string message, string channelID, CancellationToken token);
        Task<IEnumerable<T>> GetMessagesAsync<T>(string channelID, int pageSize, int offset);
        Task<IEnumerable<T>> GetMessagesAsync<T>(string channelID, int pageSize, int offset, CancellationToken token);
        Task<IEnumerable<string>> GetMessagesAsync(string channelID, int pageSize, int offset);
        Task<IEnumerable<string>> GetMessagesAsync(string channelID, int pageSize, int offset, CancellationToken token);
    }
}


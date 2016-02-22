using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tojeero.Forms.BL.Contracts;

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

		Task<List<T>> GetMessagesAsync<T>(string channelID, DateTimeOffset? startDate, int pageSize) where T : IChatMessage;
		Task<List<T>> GetMessagesAsync<T>(string channelID, DateTimeOffset? startDate, int pageSize, CancellationToken token) where T : IChatMessage;

        Task<List<IChatChannel>> FetchRecentChannelsAsync(string userID, int pageSize = -1, int offset = -1);
    }
}


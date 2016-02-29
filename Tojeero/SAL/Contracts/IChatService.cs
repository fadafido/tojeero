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

        Task SendMessageAsync(IChatMessage message, string channelID);
        Task SendMessageAsync(IChatMessage message, string channelID, CancellationToken token);

	    Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize);

	    Task<IEnumerable<IChatMessage>> GetMessagesAsync(string channelID, DateTimeOffset? startDate, int pageSize,
	        CancellationToken token);
    }
}


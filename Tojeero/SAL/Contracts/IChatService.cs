using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tojeero.Core.Services
{
	public interface IChatService
	{
	    Task SignUpAsync(IUser user);
        Task SignUpAsync(IUser user, CancellationToken token);
        Task LogInAsync(IUser user);
        Task LogInAsync(IUser user, CancellationToken token);
	    Task LogOutAsync();
        Task LogOutAsync(CancellationToken token);
        Task SubscribeToChannelAsync(IUser user, string channelID);
        Task SubscribeToChannelAsync(IUser user, string channelID, CancellationToken token);

        Task UnsubscribeFromChannelAsync(IUser user, string channelID);
        Task UnsubscribeFromChannelAsync(IUser user, string channelID, CancellationToken token);

        Task SendMessageAsync(IUser sender, IChatMessage message, string channelID);
        Task SendMessageAsync(IUser sender, IChatMessage message, string channelID, CancellationToken token);

	    Task<IEnumerable<IChatMessage>> GetMessagesAsync(IUser user, string channelID, DateTimeOffset? startDate, int pageSize);

	    Task<IEnumerable<IChatMessage>> GetMessagesAsync(IUser user, string channelID, DateTimeOffset? startDate, int pageSize,
	        CancellationToken token);
    }
}


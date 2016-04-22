using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Chat
{
    public class ChatChannelsViewModel : BaseCollectionViewModel<IChatChannel>
    {
        #region Private fields

        private readonly IChatChannelManager _chatChannelManager;

        #endregion

        #region Constructors

        public ChatChannelsViewModel(IChatChannelManager chatChannelManager)
            : base(new ChatChannelsQuery(chatChannelManager), Constants.ChatChannelsPageSize)
        {
            _chatChannelManager = chatChannelManager;
        }

        #endregion

        #region Queries

        private class ChatChannelsQuery : IModelQuery<IChatChannel>
        {
            private readonly IChatChannelManager _chatChannelManager;

            public ChatChannelsQuery(IChatChannelManager chatChannelManager)
            {
                _chatChannelManager = chatChannelManager;
            }

            public async Task<IEnumerable<IChatChannel>> Fetch(int pageSize = -1, int offset = -1)
            {
                return new List<IChatChannel>();
            }

            public Comparison<IChatChannel> Comparer => null;
            public async Task ClearCache()
            {
                await _chatChannelManager.ClearCache();
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Managers.Contracts
{
    public interface IChatChannelManager : IBaseModelEntityManager
    {
        Task<List<IChatChannel>> FetchRecentChannelsAsync(int pageSize = -1, int offset = -1);
    }
}
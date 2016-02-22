using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tojeero.Core
{
    public interface IChatChannelManager : IBaseModelEntityManager
    {
        Task<List<IChatChannel>> FetchRecentChannelsAsync(int pageSize = -1, int offset = -1);
    }
}

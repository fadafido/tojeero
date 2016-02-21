using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tojeero.Core
{
    public interface IChatResponseMessage
    {
        string ChannelID { get; set; }
        string JsonContent { get; set; }
        DateTimeOffset MessageDate { get; set; }
        T GetContent<T>();
    }

    public interface IChatResponseMessage<T> : IChatResponseMessage
    {
        T Content { get; }
    }
}

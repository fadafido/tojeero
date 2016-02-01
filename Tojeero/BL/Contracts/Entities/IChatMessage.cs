using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tojeero.Core
{
    public interface IChatMessage
    {
        string Text { get; set; }
        void Clear();
    }
}

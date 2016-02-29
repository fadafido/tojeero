using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tojeero.Forms.ViewModels.Misc;

namespace Tojeero.Forms
{
    public class ChatListView : ListView
    {
        public ChatListView()
            : base()
        {

        }

        public Action SaveScrollPosition { get; set; }
        public Action RestoreScrollPosition { get; set; }
    }
}

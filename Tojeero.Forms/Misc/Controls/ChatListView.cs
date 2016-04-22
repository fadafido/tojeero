using System;

namespace Tojeero.Forms.Controls
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

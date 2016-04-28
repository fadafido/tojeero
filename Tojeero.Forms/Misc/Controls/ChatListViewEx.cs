using System;

namespace Tojeero.Forms.Controls
{
    public class ChatListViewEx : ListViewEx
    {
        public Action SaveScrollPosition { get; set; }
        public Action RestoreScrollPosition { get; set; }
    }
}
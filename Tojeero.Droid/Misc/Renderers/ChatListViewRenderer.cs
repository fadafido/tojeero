using Tojeero.Droid.Renderers;
using Tojeero.Forms.Controls;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof (ChatListView), typeof (ChatListViewRenderer))]

namespace Tojeero.Droid.Renderers
{
    public class ChatListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
    }
}
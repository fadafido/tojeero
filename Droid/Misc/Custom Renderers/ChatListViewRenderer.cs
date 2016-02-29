using System;
using Xamarin.Forms;
using Tojeero.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using System.Linq;

[assembly: ExportRenderer(typeof(ChatListView), typeof(Tojeero.Droid.Renderers.ChatListViewRenderer))]
namespace Tojeero.Droid.Renderers
{
    public class ChatListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
    }
}
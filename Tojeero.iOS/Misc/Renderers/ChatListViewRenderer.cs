using System;
using System.ComponentModel;
using CoreGraphics;
using Tojeero.Forms.Controls;
using Tojeero.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (ChatListViewEx), typeof (ChatListViewRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class ChatListViewRenderer : ListViewRenderer
    {
        #region Private fields

        private nfloat _currentTableContentHeight;

        #endregion

        #region Parent override

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control != null && Element != null)
            {
                var chatList = Element as ChatListViewEx;
                chatList.SaveScrollPosition = () => { _currentTableContentHeight = Control.ContentSize.Height; };
                chatList.RestoreScrollPosition = () =>
                {
                    var height = Control.ContentSize.Height;
                    var afterContentOffset = Control.ContentOffset;
                    var newContentOffset = new CGPoint(afterContentOffset.X,
                        afterContentOffset.Y + height - _currentTableContentHeight);
                    Control.ContentOffset = newContentOffset;
                };
            }
        }

        #endregion
    }
}
using System;
using System.Drawing;
using Foundation;
using Tojeero.Forms.Views.Chat;
using Tojeero.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof (ChatMessageCell), typeof (ChatMessageCellRenderer))]

namespace Tojeero.iOS.Renderers
{
    public class ChatMessageCellRenderer : ViewCellRenderer
    {
        protected void OnBindingContextChanged(Cell cell)
        {
            var chatCell = cell.BindingContext as ChatMessageCell;
            var messageText = chatCell?.ViewModel?.Message?.Text;
            if (!string.IsNullOrEmpty(messageText))
            {
                var width = UIScreen.MainScreen.Bounds.Width - chatCell.TotallPadding.Width;
                var font = UIFont.FromName(chatCell.MessageLabel.FontFamily, (nfloat) chatCell.MessageLabel.FontSize);
                var height = Convert.ToDouble(EstimateHeight(messageText, (int) width, font));
                cell.Height = height + chatCell.TotallPadding.Height;
            }
            else
            {
                cell.Height = 40;
            }
        }

        private double EstimateHeight(string text, int width, UIFont font)
        {
            var size = ((NSString) text).StringSize(font, new SizeF(width, float.MaxValue), UILineBreakMode.WordWrap);
            return size.Height;
        }
    }
}
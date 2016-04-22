using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Foundation;
using Tojeero.Forms;
using Tojeero.Forms.Views.Chat;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ChatMessageCell), typeof(Tojeero.iOS.Renderers.ChatMessageCellRenderer))]

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
                var height = Convert.ToDouble(EstimateHeight(messageText, (int)width, font));
                cell.Height = height + chatCell.TotallPadding.Height;
            }
            else
            {
                cell.Height = 40;
            }
        }

        private double EstimateHeight(String text, Int32 width, UIFont font)
        {
            var size = ((NSString)text).StringSize(font, new SizeF(width, float.MaxValue), UILineBreakMode.WordWrap);
            return size.Height;
        }

    }
}

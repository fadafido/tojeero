using System;
using Android.Graphics;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Droid.Messages
{
    public class CameraImageSelectedMessage : MvxMessage
    {
        public CameraImageSelectedMessage(object sender, Guid receiverID, Bitmap image)
            : base(sender)
        {
            ReceiverID = receiverID;
            Image = image;
        }

        public Guid ReceiverID { get; set; }
        public Bitmap Image { get; set; }
    }
}
using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Android.Graphics;

namespace Tojeero.Droid
{
	public class CameraImageSelectedMessage : MvxMessage
	{
		public CameraImageSelectedMessage(object sender, Guid receiverID, Bitmap image) 
			:base(sender)
		{
			ReceiverID = receiverID;
			Image = image;
		}

		public Guid ReceiverID { get; set; }
		public Bitmap Image { get; set; }
	}
}


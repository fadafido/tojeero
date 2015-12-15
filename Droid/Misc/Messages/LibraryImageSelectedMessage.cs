using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Android.Graphics;

namespace Tojeero.Droid
{
	public class LibraryImageSelectedMessage : MvxMessage
	{
		public LibraryImageSelectedMessage(object sender, Guid receiverID, string path) 
			:base(sender)
		{
			ReceiverID = receiverID;
			Path = path;
		}

		public Guid ReceiverID { get; set; }
		public string Path { get; set; }
	}
}


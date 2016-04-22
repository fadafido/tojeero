using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Droid.Messages
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


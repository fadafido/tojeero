using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Messages
{

	public class StoreChangedMessage : MvxMessage
	{
		public StoreChangedMessage(object sender, IStore store, EntityChangeType changeType)
			:base(sender)
		{
			Store = store;
			ChangeType = changeType;
		}

		public IStore Store { get; set; }
		public EntityChangeType ChangeType { get; set; }
	}
}


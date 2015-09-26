using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core;

namespace Tojeero.Core.Messages
{
	public class CurrentUserChangedMessage : MvxMessage
	{
		#region Constructors

		public CurrentUserChangedMessage(object sender, User currentUser)
			: base(sender)
		{
			CurrentUser = currentUser;
		}

		#endregion

		#region Properties

		public User CurrentUser { get; set; }

		#endregion
	}
}


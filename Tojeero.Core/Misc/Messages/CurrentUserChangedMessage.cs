using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Messages
{
	public class CurrentUserChangedMessage : MvxMessage
	{
		#region Constructors

		public CurrentUserChangedMessage(object sender, IUser currentUser)
			: base(sender)
		{
			CurrentUser = currentUser;
		}

		#endregion

		#region Properties

		public IUser CurrentUser { get; set; }

		#endregion
	}
}


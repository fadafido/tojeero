using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public class SessionStateChangedMessage : MvxMessage
	{
		#region Constructors

		public SessionStateChangedMessage(object sender, SessionState newState)
			: base(sender)
		{
			NewState = newState;
		}

		#endregion

		#region Properties

		public SessionState NewState { get; set; }

		#endregion
	}
}


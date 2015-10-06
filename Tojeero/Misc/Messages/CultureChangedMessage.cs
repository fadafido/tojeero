using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Globalization;

namespace Tojeero.Core.Messages
{
	public class CultureChangedMessage : MvxMessage
	{
		#region Constructors

		public CultureChangedMessage(object sender, CultureInfo culture)
			: base(sender)
		{
			CurrentCulture = culture;
		}

		#endregion

		#region Properties

		public CultureInfo CurrentCulture { get; set; }

		#endregion
	}
}


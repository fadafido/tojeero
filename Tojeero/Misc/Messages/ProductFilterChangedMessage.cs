﻿using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace Tojeero.Core.Messages
{
	public class ProductFilterChangedMessage : MvxMessage
	{
		#region Constructors

		public ProductFilterChangedMessage(object sender, IProductFilter productFilter)
			: base(sender)
		{
			ProductFilter = productFilter;
		}

		#endregion

		#region Properties

		public IProductFilter ProductFilter { get; set; }

		#endregion
	}
}


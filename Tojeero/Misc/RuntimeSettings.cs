using System;
using Tojeero.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using Tojeero.Core.Messages;

namespace Tojeero.Core
{
	public static class RuntimeSettings
	{
		private static IMvxMessenger _messenger;
		
		private static IProductFilter _productFilter;
		public static IProductFilter ProductFilter
		{
			get
			{
				if (_productFilter == null)
					_productFilter = new ProductFilter();
				return _productFilter;
			}
		}
	}
}


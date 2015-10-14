using System;
using Tojeero.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using Tojeero.Core.Messages;

namespace Tojeero.Forms
{
	public static class RuntimeSettings
	{
		private static IMvxMessenger _messenger;
		
		private static IProductFilter _productFilter;
		public static IProductFilter ProductFilter
		{
			get
			{
				return _productFilter;
			}
			set
			{
				if (_productFilter != value)
				{
					_messenger = _messenger ?? Mvx.Resolve<IMvxMessenger>();
					_productFilter = value;
					_messenger.Publish<ProductFilterChangedMessage>(new ProductFilterChangedMessage(null, _productFilter));
				}
			}
		}
	}
}


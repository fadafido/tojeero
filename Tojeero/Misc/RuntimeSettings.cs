﻿using System;
using Tojeero.Core;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using Tojeero.Core.Messages;

namespace Tojeero.Core
{
	public class RuntimeSettings
	{
		
		private static IMvxMessenger _messenger;
		
		private static IProductFilter _productFilter;
		public static IProductFilter ProductFilter
		{
			get
			{
			    if (_productFilter == null)
			    {
			        _productFilter = new ProductFilter();
			    }
			    return _productFilter;
			}
			set
			{
				if (!_productFilter.Equals(value))
				{
					_productFilter = value;
					_messenger = _messenger ?? Mvx.Resolve<IMvxMessenger>();
					_messenger.Publish<ProductFilterChangedMessage>(new ProductFilterChangedMessage(new object(), value));
				}
			}
		}

		private static IStoreFilter _storeFilter;
		public static IStoreFilter StoreFilter
		{
			get
			{
				if (_storeFilter == null)
					_storeFilter = new StoreFilter();
				return _storeFilter;
			}
			set
			{
				if (!_storeFilter.Equals(value))
				{
					_storeFilter = value;
					_messenger = _messenger ?? Mvx.Resolve<IMvxMessenger>();
					_messenger.Publish<StoreFilterChangedMessage>(new StoreFilterChangedMessage(new object(), value));
				}
			}
		}
	}
}


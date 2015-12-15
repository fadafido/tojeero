using System;
using Tojeero.Core.ViewModels;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
	public static class Comparers
	{
		private static Comparison<ProductViewModel> _productName;
		public static Comparison<ProductViewModel> ProductName
		{
			get
			{
				if (_productName == null)
				{
					_productName = new Comparison<ProductViewModel>((x, y) =>
						{
							if(x.Product == null || y.Product == null)
								return -1;
							if(x.Product.ID == y.Product.ID)
								return 0;
							if(x.Product.LowercaseName == null || y.Product.LowercaseName == null)
								return -1;
							return x.Product.LowercaseName.CompareIgnoreCase(y.Product.LowercaseName);
						});
				}
				return _productName;
			}
		}

		private static Comparison<ITag> _tagText;
		public static Comparison<ITag> TagText
		{
			get
			{
				if (_tagText == null)
				{
					_tagText = new Comparison<ITag>((x, y) =>
						{
							if (x.ID == y.ID)
								return 0;
							return x.Text.CompareTo(y.Text);
						});
				}
				return _tagText;
			}
		}

		private static Comparison<StoreViewModel> _storeName;
		public static Comparison<StoreViewModel> StoreName
		{
			get
			{
				if (_storeName == null)
				{
					_storeName = new Comparison<StoreViewModel>((x, y) =>
						{
							if(x.Store == null || y.Store == null)
								return -1;
							if(x.Store.ID == y.Store.ID)
								return 0;
							if(x.Store.LowercaseName == null || y.Store.LowercaseName == null)
								return -1;
							return x.Store.LowercaseName.CompareIgnoreCase(y.Store.LowercaseName);
						});
				}
				return _storeName;
			}
		}
	}
}


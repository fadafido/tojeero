using System;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.ViewModels;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Chat;
using Tojeero.Core.ViewModels.Product;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Core.ViewModels.Tag;

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

		private static Comparison<TagViewModel> _tagText;
		public static Comparison<TagViewModel> TagText
		{
			get
			{
				if (_tagText == null)
				{
					_tagText = new Comparison<TagViewModel>((x, y) =>
						{
							if(x == null || y == null || x.Tag == null || y.Tag == null)
								return -1;
							if (x.Tag.ID == y.Tag.ID)
								return 0;
							return x.Tag.Text.CompareTo(y.Tag.Text);
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

		private static Func<IUniqueEntity, IUniqueEntity, bool> _uniqueEntityEqualityComparer;

		public static Func<IUniqueEntity, IUniqueEntity, bool> UniqueEntityEqualityComparer
		{
			get
			{
				if (_uniqueEntityEqualityComparer == null)
				{
					_uniqueEntityEqualityComparer = (x, y) =>
						{
							if(x == null || y == null)
								return false;
							return x == y || x.ID == y.ID;
						};
				}
				return _uniqueEntityEqualityComparer;
			}
		}

        private static Comparison<ChatMessageViewModel> _chatMessage;
        public static Comparison<ChatMessageViewModel> ChatMessage
        {
            get
            {
                if (_chatMessage == null)
                {
                    _chatMessage = new Comparison<ChatMessageViewModel>((x, y) =>
                    {
                        if (x?.Message?.DeliveryDate == null || y?.Message?.DeliveryDate == null)
                            return -1;
                        var res = x.Message.DeliveryDate.Value.CompareTo(y.Message.DeliveryDate.Value);
                        return res != 0 ? res : -1;
                    });
                }
                return _chatMessage;
            }
        }
    }
}


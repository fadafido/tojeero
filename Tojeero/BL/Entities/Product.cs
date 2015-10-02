using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	public class Product : BaseModelEntity<ParseProduct>, IProduct
	{
		#region Constructors

		public Product()
			:base()
		{
		}

		public Product(ParseProduct parseProduct = null)
			: base(parseProduct)
		{
			
		}

		#endregion

		#region Properties

		[Ignore]
		public override ParseProduct ParseObject
		{
			get
			{
				return base.ParseObject;
			}
			set
			{
				var newValue = value;
				if (value == null)
					newValue = Parse.ParseObject.Create<ParseProduct>();
				_imageUrl = null;
				base.ParseObject = value;
			}
		}

		public string Name
		{
			get
			{
				return _parseObject.Name;
			}
			set
			{
				_parseObject.Name = value;
				RaisePropertyChanged(() => Name);
			}
		}
			
		public double Price
		{
			get
			{
				return _parseObject.Price;
			}
			set
			{
				_parseObject.Price = value;
				RaisePropertyChanged(() => Price);
				RaisePropertyChanged(() => FormattedPrice);
			}
		}

		[Ignore]
		public string FormattedPrice
		{
			get
			{
				return Price.ToString("C");
			}
		}

		private string _imageUrl;
		public string ImageUrl
		{
			get
			{
				if (_imageUrl == null)
					_imageUrl = _parseObject.ImageUrl;
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
				RaisePropertyChanged(() => ImageUrl);
			}
		}

		#endregion
	}

	[ParseClassName("StoreItem")]
	public class ParseProduct : ParseObject
	{
		#region Constructors

		public ParseProduct()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("price")]
		public double Price
		{
			get
			{
				return GetProperty<double>();
			}
			set
			{
				SetProperty<double>(value);
			}
		}

		public string FormattedPrice
		{
			get
			{
				return Price.ToString("C");
			}
		}

		public string ImageUrl
		{
			get
			{
				return Image != null ? Image.Url.ToString() : null;
			}
		}

		[ParseFieldName("image")]
		public ParseFile Image
		{
			get
			{
				return GetProperty<ParseFile>();
			}
			set
			{
				SetProperty<ParseFile>(value);
			}
		}

		#endregion
	}
}


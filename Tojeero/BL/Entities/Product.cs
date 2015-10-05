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
				_imageUri = null;
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
				if (_imageUrl == null && ImageUri != null)
					_imageUrl = ImageUri.ToString();
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
				ImageUri = value != null ? new Uri(value) : null;
			}
		}

		private Uri _imageUri;
		[Ignore]
		public Uri ImageUri
		{ 
			get
			{
				if (_imageUri == null && _parseObject != null && _parseObject.Image != null)
				{
					_imageUri = _parseObject.Image.Url;
				}
				return _imageUri; 
			}
			set
			{
				_imageUri = value; 
				RaisePropertyChanged(() => ImageUri); 
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


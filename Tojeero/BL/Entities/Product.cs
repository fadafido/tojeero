using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class Product : BaseModelEntity<ParseProduct>, IProduct
	{
		#region Constructors

		public Product()
			: base()
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
				_imageUrl = null;
				_category = null;
				_subcategory = null;
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

		public string LowercaseName
		{
			get
			{
				return _parseObject.LowercaseName;
			}
			set
			{
				_parseObject.LowercaseName = value;
				RaisePropertyChanged(() => LowercaseName);
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
				return Price.ToString("N2");
			}
		}

		private string _imageUrl;

		public string ImageUrl
		{
			get
			{
				if (_imageUrl == null && _parseObject != null && _parseObject.Image != null && _parseObject.Image.Url != null)
					_imageUrl = _parseObject.Image.Url.ToString();
				return _imageUrl;
			}
			set
			{
				_imageUrl = value;
				RaisePropertyChanged(() => ImageUrl); 
			}
		}

		private string _categoryID;

		public string CategoryID
		{ 
			get
			{
				if (_categoryID == null && _parseObject != null && _parseObject.Category != null)
					_categoryID = _parseObject.Category.ObjectId;
				return _categoryID; 
			}
			set
			{
				_categoryID = value; 
			}
		}

		private string _subcategoryID;

		public string SubcategoryID
		{ 
			get
			{
				if (_subcategoryID == null && _parseObject != null && _parseObject.Subcategory != null)
					_subcategoryID = _parseObject.Subcategory.ObjectId;
				return _subcategoryID; 
			}
			set
			{
				_subcategoryID = value; 
			}
		}

		public int? CityId
		{
			get
			{
				return this.ParseObject.CityId;
			}
			set
			{
				this.ParseObject.CityId = value;
				this.RaisePropertyChanged(() => CityId);
			}
		}

		public int? CountryId
		{
			get
			{
				return this.ParseObject.CountryId;
			}
			set
			{
				this.ParseObject.CountryId = value;
				this.RaisePropertyChanged(() => CountryId);
			}
		}

		[Ignore]
		public IList<string> SearchTokens
		{
			get
			{
				return _parseObject != null ? _parseObject.SearchTokens : null;
			}
			set
			{
				if (_parseObject != null)
				{
					_parseObject.SearchTokens = value;
				}
			}
		}

		[Ignore]
		public IList<string> Tags
		{
			get
			{
				return _parseObject != null ? _parseObject.Tags : null;
			}
			set
			{
				if (_parseObject != null)
				{
					_parseObject.Tags = value;
					_tagList = null;
					RaisePropertyChanged(() => Tags);
					RaisePropertyChanged(() => TagList);
				}
			}
		}


		private string _tagList;
		[Ignore]
		public string TagList
		{
			get
			{
				if (_tagList == null)
				{

					if (Tags != null)
						_tagList = string.Join(", ", Tags);
					else
						_tagList = "";
				}
				return _tagList;
			}
		}

		private IProductCategory _category;
		[Ignore]
		public IProductCategory Category
		{ 
			get
			{
				if (_category == null)
					_category = new ProductCategory(this.ParseObject.Category);
				return _category; 
			}
		}

		private IProductSubcategory _subcategory;
		[Ignore]
		public IProductSubcategory Subcategory
		{ 
			get
			{
				if (_subcategory == null)
					_subcategory = new ProductSubcategory(this.ParseObject.Subcategory);
				return _subcategory; 
			}
		}
			
		private bool? _isFavorite;
		public bool? IsFavorite
		{ 
			get
			{
				return _isFavorite; 
			}
			set
			{
				_isFavorite = value; 
				RaisePropertyChanged(() => IsFavorite); 
			}
		}

		#endregion

		#region Parent

		public override string ToString()
		{
			return this.ID + " " + this.Name;	
		}

		#endregion
	}

	[ParseClassName("StoreItem")]
	public class ParseProduct : SearchableParseObject
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

		[ParseFieldName("lowercase_name")]
		public string LowercaseName
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

		[ParseFieldName("description")]
		public string Description
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

		[ParseFieldName("tags")]
		public IList<string> Tags
		{
			get
			{
				return GetProperty<IList<string>>();
			}
			set
			{
				SetProperty<IList<string>>(value);
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

		[ParseFieldName("category")]
		public ParseProductCategory Category
		{
			get
			{
				return GetProperty<ParseProductCategory>();
			}
			set
			{
				SetProperty<ParseProductCategory>(value);
			}
		}

		[ParseFieldName("subcategory")]
		public ParseProductSubcategory Subcategory
		{
			get
			{
				return GetProperty<ParseProductSubcategory>();
			}
			set
			{
				SetProperty<ParseProductSubcategory>(value);
			}
		}

		[ParseFieldName("cityId")]
		public int? CityId
		{
			get
			{
				return GetProperty<int?>();
			}
			set
			{
				SetProperty<int?>(value);
			}
		}

		[ParseFieldName("countryId")]
		public int? CountryId
		{
			get
			{
				return GetProperty<int?>();
			}
			set
			{
				SetProperty<int?>(value);
			}
		}

		#endregion
	}
}


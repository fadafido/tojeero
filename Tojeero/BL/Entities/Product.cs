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
				_store = null;
				_country = null;
				_city = null;
				_isFavorite = null;
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

		public string Description
		{
			get
			{
				return _parseObject.Description;
			}
			set
			{
				_parseObject.Description = value;
				RaisePropertyChanged(() => Description);
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

		private string _storeID;

		public string StoreID
		{ 
			get
			{
				if (_storeID == null && _parseObject != null && _parseObject.Store != null)
					_storeID = _parseObject.Store.ObjectId;
				return _storeID; 
			}
			set
			{
				_storeID = value; 
			}
		}

		private string _cityId;

		public string CityId
		{
			get
			{
				if (_cityId == null && _parseObject != null && _parseObject.City != null)
					_cityId = _parseObject.City.ObjectId;
				return _cityId;
			}
			set
			{
				if (_cityId != value)
				{
					_cityId = value;
					_city = null;
					this.ParseObject.City = Parse.ParseObject.CreateWithoutData<ParseCity>(_cityId);
				}
			}
		}

		private string _countryId;

		public string CountryId
		{
			get
			{
				if (_countryId == null && _parseObject != null && _parseObject.Country != null)
					_countryId = _parseObject.Country.ObjectId;
				return _countryId;
			}
			set
			{
				if (_countryId != value)
				{
					_countryId = value;
					_country = null;
					this.ParseObject.Country = Parse.ParseObject.CreateWithoutData<ParseCountry>(_countryId);
				}
			}
		}

		private ICountry _country;
		[Ignore]
		public ICountry Country
		{ 
			get
			{
				if (_country == null)
					_country = new Country(this.ParseObject.Country);
				return _country; 
			}
		}

		private ICity _city;
		[Ignore]
		public ICity City
		{ 
			get
			{
				if (_city == null)
					_city = new City(this.ParseObject.City);
				return _city; 
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

		private IStore _store;
		[Ignore]
		public IStore Store
		{ 
			get
			{
				if (_store == null)
					_store = new Store(this.ParseObject.Store);
				return _store; 
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

		[ParseFieldName("store")]
		public ParseStore Store
		{
			get
			{
				return GetProperty<ParseStore>();
			}
			set
			{
				SetProperty<ParseStore>(value);
			}
		}

		[ParseFieldName("country")]
		public ParseCountry Country
		{
			get
			{
				return GetProperty<ParseCountry>();
			}
			set
			{
				SetProperty<ParseCountry>(value);
			}
		}

		[ParseFieldName("city")]
		public ParseCity City
		{
			get
			{
				return GetProperty<ParseCity>();
			}
			set
			{
				SetProperty<ParseCity>(value);
			}
		}

		#endregion
	}
}


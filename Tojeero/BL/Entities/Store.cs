using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;

namespace Tojeero.Core
{
	public class Store : BaseModelEntity<ParseStore>, IStore
	{
		#region Constructors

		public Store()
			:base()
		{
			
		}

		public Store(ParseStore parseStore = null)
			: base(parseStore)
		{

		}

		#endregion

		#region Properties

		[Ignore]
		public override ParseStore ParseObject
		{
			get
			{
				return base.ParseObject;
			}
			set
			{
				_imageUrl = null;
				_category = null;
				_isFavorite = null;
				_country = null;
				_city = null;
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
			
		public string Description
		{
			get
			{
				return this.ParseObject.Description;
			}
			set
			{
				this.ParseObject.Description = value;
				RaisePropertyChanged(() => Description);
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

		private IStoreCategory _category;
		[Ignore]
		public IStoreCategory Category
		{ 
			get
			{
				if (_category == null)
					_category = new StoreCategory(this.ParseObject.Category);
				return _category; 
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

		#region Methods

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			var manager = Mvx.Resolve<IModelEntityManager>();
			var result = await manager.Fetch<IProduct, Product>(new StoreProductsQueryLoader(pageSize, offset, manager, this), Constants.ProductsCacheTimespan.TotalMilliseconds);
			return result;
		}

		#endregion	

	}
		
	[ParseClassName("Store")]
	public class ParseStore : SearchableParseObject
	{
		#region Constructors

		public ParseStore()
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
		public ParseStoreCategory Category
		{
			get
			{
				return GetProperty<ParseStoreCategory>();
			}
			set
			{
				SetProperty<ParseStoreCategory>(value);
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


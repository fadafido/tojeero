﻿using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Nito.AsyncEx;
using Newtonsoft.Json;

namespace Tojeero.Core
{
	public class Store : BaseModelEntity<ParseStore>, IStore
	{
		#region Private fields and properties

		AsyncReaderWriterLock _countryLocker = new AsyncReaderWriterLock();

		#endregion

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
				_country = null;
				_city = null;
				_owner = null;
				base.ParseObject = value;
			}
		}

		[JsonProperty("name")]
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

		[JsonProperty("lowercase_name")]
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
			
		[JsonProperty("description")]
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

		[JsonProperty("deliveryNotes")]
		public string DeliveryNotes
		{
			get
			{
				return this.ParseObject.DeliveryNotes;
			}
			set
			{
				this.ParseObject.DeliveryNotes = value;
				RaisePropertyChanged(() => DeliveryNotes);
			}
		}

		private string _imageUrl;
		[JsonProperty("imageUrl")]
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

		[JsonProperty("isBlocked")]
		public bool IsBlocked
		{
			get
			{
				return _parseObject.IsBlocked;
			}
			set
			{
				_parseObject.IsBlocked = value;
				RaisePropertyChanged(() => IsBlocked);
			}
		}

		private string _categoryID;
		[JsonProperty("categoryID")]
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
				if (_categoryID != value)
				{
					_categoryID = value;
					_category = null;
					this.ParseObject.Category = value != null ? Parse.ParseObject.CreateWithoutData<ParseStoreCategory>(value) : null;
					RaisePropertyChanged(() => Category);
				}
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
		[JsonProperty("cityID")]
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
					this.ParseObject.City = value != null ? Parse.ParseObject.CreateWithoutData<ParseCity>(value) : null;
					RaisePropertyChanged(() => City);
				}
			}
		}

		private string _countryId;
		[JsonProperty("countryID")]
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
					this.ParseObject.Country = value != null ? Parse.ParseObject.CreateWithoutData<ParseCountry>(value) : null;
					RaisePropertyChanged(() => Country);
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
					RaisePropertyChanged(() => SearchTokens);
				}
			}
		}

		private string _ownerID;
		[JsonProperty("ownerID")]
		public string OwnerID
		{
			get
			{
				if (_ownerID == null && _parseObject != null && _parseObject.Owner != null)
					_ownerID = _parseObject.Owner.ObjectId;
				return _ownerID;
			}
			set
			{
				if (_ownerID != value)
				{
					_ownerID = value;
					_owner = null;
					this.ParseObject.Owner = value != null ? Parse.ParseObject.CreateWithoutData<TojeeroUser>(value) : null;
					RaisePropertyChanged(() => Owner);
				}
			}
		}

		private IUser _owner;
		[Ignore]
		public IUser Owner
		{ 
			get
			{
				if (_owner == null)
					_owner = new User(this.ParseObject.Owner);
				return _owner; 
			}
		}
		#endregion

		#region Methods

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, bool includeInvisible = false)
		{
			var manager = Mvx.Resolve<IModelEntityManager>();
			var result = await manager.Fetch<IProduct, Product>(new StoreProductsQueryLoader(pageSize, offset, manager, this, includeInvisible), Constants.ProductsCacheTimespan.TotalMilliseconds);
			return result;
		}

		public async Task Save()
		{
			if (this.ParseObject != null)
			{
				await this.ParseObject.SaveAsync();
				var query = new ParseQuery<ParseStore>().Where(s => s.ObjectId == this.ParseObject.ObjectId).Include("category").Include("country").Include("city");
				var store = await query.FirstOrDefaultAsync();
				this.ParseObject = store;
			}
		}

		public async Task SetMainImage(IImage image)
		{
			var imageFile = new ParseFile(image.Name, image.RawImage);
			await imageFile.SaveAsync();
			this.ParseObject.Image = imageFile;
			this.ImageUrl = null;
		}

		public async Task FetchCountry()
		{
			using (var writerLock = await _countryLocker.WriterLockAsync())
			{

				if (this.Country == null)
					return;
				if (this.Country.Name != null)
					return;
				var country = this.Country as Country;
				if (country != null)
				{
					await country.ParseObject.FetchAsync();
					this.RaisePropertyChanged(() => Country);
				}
			}
		}

		public async Task LoadRelationships()
		{
			if (!string.IsNullOrEmpty(this.ID))
			{
				var query = new ParseQuery<ParseStore>().Where(s => s.ObjectId == this.ID).Include("category").Include("country").Include("city");
				var store = await query.FirstOrDefaultAsync();
				this.ParseObject = store;
			}
		}

		#endregion	

	}
		
	[ParseClassName("Store")]
	public class ParseStore : ParseObject
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

		[ParseFieldName("deliveryNotes")]
		public string DeliveryNotes
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

		[ParseFieldName("isBlocked")]
		public bool IsBlocked
		{
			get
			{
				return GetProperty<bool>();
			}
			set
			{
				SetProperty<bool>(value);
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

		[ParseFieldName("owner")]
		public TojeeroUser Owner
		{
			get
			{
				return GetProperty<TojeeroUser>();
			}
			set
			{
				SetProperty<TojeeroUser>(value);
			}
		}

		[ParseFieldName("searchTokens")]
		public IList<string> SearchTokens
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
		#endregion
	}
}


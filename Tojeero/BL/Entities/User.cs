using System;
using Newtonsoft.Json;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nito.AsyncEx;
using Parse;
using System.Linq;
using Cirrious.CrossCore;

namespace Tojeero.Core
{
	public class User : BaseModelEntity<TojeeroUser>, IUser
	{
		#region Private fields and properties

		private List<string> _favoriteProducts;
		private List<string> _favoriteStores;
		private AsyncReaderWriterLock _productLocker = new AsyncReaderWriterLock();
		private AsyncReaderWriterLock _storeLocker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public User(TojeeroUser user = null)
			: base(user)
		{
		}

		#endregion

		#region Properties

		[JsonIgnore]
		public TojeeroUser ParseObject
		{ 
			get
			{
				return base.ParseObject;
			}
			set
			{
				base.ParseObject = value;
			}
		}


		[JsonProperty("first_name")]
		public string FirstName
		{ 
			get
			{
				return this.ParseObject.FirstName; 
			}
			set
			{
				this.ParseObject.FirstName = value;
				RaisePropertyChanged(() => FirstName); 
				RaisePropertyChanged(() => FullName); 
			}
		}
			
		[JsonProperty("last_name")]
		public string LastName
		{ 
			get
			{
				return this.ParseObject.LastName; 
			}
			set
			{
				this.ParseObject.LastName = value;
				RaisePropertyChanged(() => LastName); 
				RaisePropertyChanged(() => FullName); 
			}
		}

		public string FullName
		{ 
			get
			{
				return string.Format("{0} {1}", FirstName, LastName); 
			}
		}


		public string UserName
		{ 
			get
			{
				return this.ParseObject.Username; 
			}
			set
			{
				this.ParseObject.Username = value;
				RaisePropertyChanged(() => UserName); 
			}
		}
			
		[JsonProperty("id")]
		public string ID
		{ 
			get
			{
				return base.ID; 
			}
			set
			{
				base.ID = value;
			}
		}


		[JsonProperty("email")]
		public string Email
		{ 
			get
			{
				return this.ParseObject.Email;; 
			}
			set
			{
				this.ParseObject.Email = value;
				RaisePropertyChanged(() => Email); 
			}
		}
			
		public string ProfilePictureUrl
		{ 
			get
			{
				return this.ParseObject.ProfilePictureUrl; 
			}
			set
			{
				this.ParseObject.ProfilePictureUrl = value; 
				RaisePropertyChanged(() => ProfilePictureUrl); 
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
		[JsonIgnore]
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
		[JsonIgnore]
		public ICity City
		{ 
			get
			{
				if (_city == null)
					_city = new City(this.ParseObject.City);
				return _city; 
			}
		}

		public string Mobile
		{ 
			get
			{
				return this.ParseObject.Mobile; 
			}
			set
			{
				this.ParseObject.Mobile = value; 
				RaisePropertyChanged(() => Mobile); 
			}
		}

		public bool IsProfileSubmitted 
		{
			get
			{
				return this.ParseObject.IsProfileSubmitted;
			}
			set
			{ 
				this.ParseObject.IsProfileSubmitted = value;
				RaisePropertyChanged(() => IsProfileSubmitted);
			}
		}

		private IStore _defaultStore;
		[JsonIgnore]
		public IStore DefaultStore
		{
			get
			{
				return _defaultStore;
			}
			private set
			{
				_defaultStore = value;
				RaisePropertyChanged(() => DefaultStore);
			}
		}

		#endregion

		#region Methods

		public async Task<bool> IsProductFavorite(string productID)
		{
			await loadFavoriteProductsIfNeeded();
			var result = _favoriteProducts.Contains(productID);
			return result;
		}

		public async Task AddProductToFavorites(string productID)
		{
			var user = ParseUser.CurrentUser as TojeeroUser;
			user.FavoriteProducts.Add(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
			await user.SaveAsync();
			if (_favoriteProducts != null)
				_favoriteProducts.Add(productID);
		}

		public async Task RemoveProductFromFavorites(string productID)
		{
			var user = ParseUser.CurrentUser as TojeeroUser;
			user.FavoriteProducts.Remove(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
			await user.SaveAsync();
			if (_favoriteProducts != null)
				_favoriteProducts.Remove(productID);
		}

		public async Task<bool> IsStoreFavorite(string storeID)
		{
			await loadFavoriteStoresIfNeeded();
			var result = _favoriteStores.Contains(storeID);
			return result;
		}

		public async Task AddStoreToFavorites(string storeID)
		{
			var user = ParseUser.CurrentUser as TojeeroUser;
			user.FavoriteStores.Add(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
			await user.SaveAsync();
			if (_favoriteStores != null)
				_favoriteStores.Add(storeID);
		}

		public async Task RemoveStoreFromFavorites(string storeID)
		{
			var user = ParseUser.CurrentUser as TojeeroUser;
			user.FavoriteStores.Remove(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
			await user.SaveAsync();
			if (_favoriteStores != null)
				_favoriteStores.Remove(storeID);
		}

		public async Task LoadDefaultStore()
		{
			var repo = Mvx.Resolve<IRestRepository>();
			var store = await repo.FetchDefaultStoreForUser(ParseUser.CurrentUser.ObjectId);
			this.DefaultStore = store;
		}

		#endregion

		#region Utility methods

		async Task loadFavoriteProductsIfNeeded()
		{
			using (var writerLock = await _productLocker.WriterLockAsync())
			{
				if (_favoriteProducts == null)
				{
					var user = ParseUser.CurrentUser as TojeeroUser;
					var result = await user.FavoriteProducts.Query.FindAsync();	
					_favoriteProducts = result.Select(p => p.ObjectId).ToList();
				}
			}
		}

		async Task loadFavoriteStoresIfNeeded()
		{
			using (var writerLock = await _storeLocker.WriterLockAsync())
			{
				if (_favoriteStores == null)
				{
					var user = ParseUser.CurrentUser as TojeeroUser;
					var result = await user.FavoriteStores.Query.FindAsync();	
					_favoriteStores = result.Select(p => p.ObjectId).ToList();
				}
			}
		}

		#endregion
	}

	[ParseClassName("_User")]
	public class TojeeroUser : ParseUser
	{
		[ParseFieldName("favoriteProducts")]
		public ParseRelation<ParseProduct> FavoriteProducts
		{
			get 
			{ 
				return GetRelationProperty<ParseProduct>(); 
			}
		}

		[ParseFieldName("favoriteStores")]
		public ParseRelation<ParseStore> FavoriteStores
		{
			get 
			{ 
				return GetRelationProperty<ParseStore>(); 
			}
		}

		[ParseFieldName("firstName")]
		public string FirstName
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

		[ParseFieldName("lastName")]
		public string LastName
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

		[ParseFieldName("profilePictureUri")]
		public string ProfilePictureUrl
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

		[ParseFieldName("mobile")]
		public string Mobile
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

		[ParseFieldName("isProfileSubmitted")]
		public bool IsProfileSubmitted
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
	}
}


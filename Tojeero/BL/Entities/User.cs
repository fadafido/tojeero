using System;
using Newtonsoft.Json;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nito.AsyncEx;
using Parse;
using System.Linq;

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
			
		public int? CountryId
		{ 
			get
			{
				return this.ParseObject.CountryId; 
			}
			set
			{
				this.ParseObject.CountryId = value; 
				RaisePropertyChanged(() => CountryId); 
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
				RaisePropertyChanged(() => CityId); 
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

		#endregion

		#region Methods

		public async Task<bool> IsProductFavorite(string productID)
		{
			using (var writerLock = await _productLocker.WriterLockAsync())
			{
				await loadFavoritesIfNeeded();
				var result = _favoriteProducts.Contains(productID);
				return result;
			}
		}

		public async Task AddProductToFavorites(string productID)
		{
			using (var writerLock = await _productLocker.WriterLockAsync())
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				user.FavoriteProducts.Add(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
				await user.SaveAsync();
				if (_favoriteProducts != null)
					_favoriteProducts.Add(productID);
			}
		}

		public async Task RemoveProductFromFavorites(string productID)
		{
			using (var writerLock = await _productLocker.WriterLockAsync())
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				user.FavoriteProducts.Remove(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
				await user.SaveAsync();
				if (_favoriteProducts != null)
					_favoriteProducts.Remove(productID);
			}
		}

		public async Task<bool> IsStoreFavorite(string storeID)
		{
			using (var writerLock = await _storeLocker.WriterLockAsync())
			{
				await loadFavoritesIfNeeded();
				var result = _favoriteStores.Contains(storeID);
				return result;
			}
		}

		public async Task AddStoreToFavorites(string storeID)
		{
			using (var writerLock = await _storeLocker.WriterLockAsync())
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				user.FavoriteStores.Add(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
				await user.SaveAsync();
				if (_favoriteStores != null)
					_favoriteStores.Add(storeID);
			}
		}

		public async Task RemoveStoreFromFavorites(string storeID)
		{
			using (var writerLock = await _storeLocker.WriterLockAsync())
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				user.FavoriteStores.Remove(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
				await user.SaveAsync();
				if (_favoriteStores != null)
					_favoriteStores.Remove(storeID);
			}
		}

		#endregion

		#region Utility methods

		async Task loadFavoritesIfNeeded()
		{
			if (_favoriteProducts == null)
			{
				var user = ParseUser.CurrentUser;
				var relation = user.GetRelation<ParseProduct>("favoriteProducts");
				var result = await relation.Query.FindAsync();	
				_favoriteProducts = result.Select(p => p.ObjectId).ToList();
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


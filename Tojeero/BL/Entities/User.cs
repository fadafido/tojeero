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
	public class User : MvxViewModel, IUser
	{
		#region Private fields and properties

		private List<string> _favoriteProducts;
		private AsyncReaderWriterLock _productLocker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public User()
			: base()
		{
		}

		#endregion

		#region Properties

		private string _firstName;
		[JsonProperty("first_name")]
		public string FirstName
		{ 
			get
			{
				return _firstName; 
			}
			set
			{
				_firstName = value; 
				RaisePropertyChanged(() => FirstName); 
				RaisePropertyChanged(() => FullName); 
			}
		}
			
		private string _lastName;
		[JsonProperty("last_name")]
		public string LastName
		{ 
			get
			{
				return _lastName; 
			}
			set
			{
				_lastName = value; 
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

		private string _userName;

		public string UserName
		{ 
			get
			{
				return _userName; 
			}
			set
			{
				_userName = value; 
				RaisePropertyChanged(() => UserName); 
			}
		}
			
		private string _id;
		[JsonProperty("id")]
		public string ID
		{ 
			get
			{
				return _id; 
			}
			set
			{
				_id = value; 
				RaisePropertyChanged(() => ID); 
			}
		}
			
		private string _email;
		[JsonProperty("email")]
		public string Email
		{ 
			get
			{
				return _email; 
			}
			set
			{
				_email = value; 
				RaisePropertyChanged(() => Email); 
			}
		}

		private string _profilePictureUrl;
		public string ProfilePictureUrl
		{ 
			get
			{
				return _profilePictureUrl; 
			}
			set
			{
				_profilePictureUrl = value; 
				RaisePropertyChanged(() => ProfilePictureUrl); 
			}
		}

		private int? _countryId;
		public int? CountryId
		{ 
			get
			{
				return _countryId; 
			}
			set
			{
				_countryId = value; 
				RaisePropertyChanged(() => CountryId); 
			}
		}

		private int? _cityId;
		public int? CityId
		{ 
			get
			{
				return _cityId; 
			}
			set
			{
				_cityId = value; 
				RaisePropertyChanged(() => CityId); 
			}
		}

		private string _mobile;
		public string Mobile
		{ 
			get
			{
				return _mobile; 
			}
			set
			{
				_mobile = value; 
				RaisePropertyChanged(() => Mobile); 
			}
		}

		public bool IsProfileSubmitted { get; set; }
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
				var user = ParseUser.CurrentUser;
				var relation = user.GetRelation<ParseProduct>("favoriteProducts");
				relation.Add(ParseObject.CreateWithoutData<ParseProduct>(productID));
				await user.SaveAsync();
				if (_favoriteProducts != null)
					_favoriteProducts.Add(productID);
			}
		}

		public async Task RemoveProductFromFavorites(string productID)
		{
			using (var writerLock = await _productLocker.WriterLockAsync())
			{
				var user = ParseUser.CurrentUser;
				var relation = user.GetRelation<ParseProduct>("favoriteProducts");
				relation.Remove(ParseObject.CreateWithoutData<ParseProduct>(productID));
				await user.SaveAsync();
				if (_favoriteProducts != null)
					_favoriteProducts.Remove(productID);
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
}


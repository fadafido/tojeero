using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Parse;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;

namespace Tojeero.Core.Model
{
    public class User : BaseModelEntity<TojeeroUser>, IUser
    {
        #region Private fields and properties

        private List<IFavorite> _favoriteProducts;
        private List<IFavorite> _favoriteStores;
        private readonly AsyncReaderWriterLock _productLocker = new AsyncReaderWriterLock();
        private readonly AsyncReaderWriterLock _storeLocker = new AsyncReaderWriterLock();

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
            get { return base.ParseObject; }
            set { base.ParseObject = value; }
        }


        [JsonProperty("first_name")]
        public string FirstName
        {
            get { return ParseObject.FirstName; }
            set
            {
                ParseObject.FirstName = value;
                RaisePropertyChanged(() => FirstName);
                RaisePropertyChanged(() => FullName);
            }
        }

        [JsonProperty("last_name")]
        public string LastName
        {
            get { return ParseObject.LastName; }
            set
            {
                ParseObject.LastName = value;
                RaisePropertyChanged(() => LastName);
                RaisePropertyChanged(() => FullName);
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName); }
        }


        public string UserName
        {
            get { return ParseObject.Username; }
            set
            {
                ParseObject.Username = value;
                RaisePropertyChanged(() => UserName);
            }
        }

        [JsonProperty("id")]
        public string ID
        {
            get { return base.ID; }
            set { base.ID = value; }
        }


        [JsonProperty("email")]
        public string Email
        {
            get
            {
                return ParseObject.Email;
                ;
            }
            set
            {
                ParseObject.Email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        public string ProfilePictureUrl
        {
            get { return ParseObject.ProfilePictureUrl; }
            set
            {
                ParseObject.ProfilePictureUrl = value;
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
                    ParseObject.City = Parse.ParseObject.CreateWithoutData<ParseCity>(_cityId);
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
                    ParseObject.Country = Parse.ParseObject.CreateWithoutData<ParseCountry>(_countryId);
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
                    _country = new Country(ParseObject.Country);
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
                    _city = new City(ParseObject.City);
                return _city;
            }
        }

        public string Mobile
        {
            get { return ParseObject.Mobile; }
            set
            {
                ParseObject.Mobile = value;
                RaisePropertyChanged(() => Mobile);
            }
        }

        public bool IsProfileSubmitted
        {
            get { return ParseObject.IsProfileSubmitted; }
            set
            {
                ParseObject.IsProfileSubmitted = value;
                RaisePropertyChanged(() => IsProfileSubmitted);
            }
        }

        private IStore _defaultStore;

        [JsonIgnore]
        public IStore DefaultStore
        {
            get { return _defaultStore; }
            private set
            {
                _defaultStore = value;
                RaisePropertyChanged(() => DefaultStore);
            }
        }

        #endregion

        #region Methods

        public async Task<IFavorite> GetProductFavorite(string productID)
        {
            await loadFavoriteProductsIfNeeded();
            var favorite = getProductFavorite(productID);
            return favorite;
        }

        public async Task<IFavorite> AddProductToFavorites(string productID)
        {
            var user = ParseUser.CurrentUser as TojeeroUser;
            user.FavoriteProducts.Add(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
            await user.SaveAsync();
            var favorite = getProductFavorite(productID);
            favorite.IsFavorite = true;
            return favorite;
        }

        public async Task RemoveProductFromFavorites(string productID)
        {
            var user = ParseUser.CurrentUser as TojeeroUser;
            user.FavoriteProducts.Remove(Parse.ParseObject.CreateWithoutData<ParseProduct>(productID));
            await user.SaveAsync();
            var favorite = getProductFavorite(productID);
            favorite.IsFavorite = false;
        }

        public async Task<IFavorite> GetStoreFavorite(string storeID)
        {
            await loadFavoriteStoresIfNeeded();
            var favorite = getStoreFavorite(storeID);
            return favorite;
        }

        public async Task<IFavorite> AddStoreToFavorites(string storeID)
        {
            var user = ParseUser.CurrentUser as TojeeroUser;
            user.FavoriteStores.Add(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
            await user.SaveAsync();
            var favorite = getStoreFavorite(storeID);
            favorite.IsFavorite = true;
            return favorite;
        }

        public async Task RemoveStoreFromFavorites(string storeID)
        {
            var user = ParseUser.CurrentUser as TojeeroUser;
            user.FavoriteStores.Remove(Parse.ParseObject.CreateWithoutData<ParseStore>(storeID));
            await user.SaveAsync();
            var favorite = getStoreFavorite(storeID);
            favorite.IsFavorite = false;
        }

        public async Task LoadDefaultStore()
        {
            if (DefaultStore != null)
                return;
            var repo = Mvx.Resolve<IRestRepository>();
            var store = await repo.FetchDefaultStoreForUser(ParseUser.CurrentUser.ObjectId);
            DefaultStore = store;
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
                    _favoriteProducts = result.Select(p => new Favorite(p.ObjectId, true)).ToList<IFavorite>();
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
                    _favoriteStores = result.Select(p => new Favorite(p.ObjectId, true)).ToList<IFavorite>();
                }
            }
        }

        private IFavorite getProductFavorite(string productID)
        {
            var favorite = _favoriteProducts.Where(f => f.ObjectID == productID).FirstOrDefault();
            if (favorite == null)
            {
                favorite = new Favorite(productID);
                _favoriteProducts.Add(favorite);
            }
            return favorite;
        }

        private IFavorite getStoreFavorite(string storeID)
        {
            var favorite = _favoriteStores.Where(f => f.ObjectID == storeID).FirstOrDefault();
            if (favorite == null)
            {
                favorite = new Favorite(storeID);
                _favoriteStores.Add(favorite);
            }
            return favorite;
        }

        #endregion
    }

    [ParseClassName("_User")]
    public class TojeeroUser : ParseUser
    {
        [ParseFieldName("favoriteProducts")]
        public ParseRelation<ParseProduct> FavoriteProducts
        {
            get { return GetRelationProperty<ParseProduct>(); }
        }

        [ParseFieldName("favoriteStores")]
        public ParseRelation<ParseStore> FavoriteStores
        {
            get { return GetRelationProperty<ParseStore>(); }
        }

        [ParseFieldName("firstName")]
        public string FirstName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("lastName")]
        public string LastName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("profilePictureUri")]
        public string ProfilePictureUrl
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("country")]
        public ParseCountry Country
        {
            get { return GetProperty<ParseCountry>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("city")]
        public ParseCity City
        {
            get { return GetProperty<ParseCity>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("mobile")]
        public string Mobile
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("isProfileSubmitted")]
        public bool IsProfileSubmitted
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Parse;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Queries;

namespace Tojeero.Core.Model
{
    public class Store : BaseModelEntity<ParseStore>, IStore
    {
        #region Private fields and properties

        readonly AsyncReaderWriterLock _countryLocker = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public Store()
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
            get { return base.ParseObject; }
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
            get { return _parseObject.Name; }
            set
            {
                _parseObject.Name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        [JsonProperty("lowercase_name")]
        public string LowercaseName
        {
            get { return _parseObject.LowercaseName; }
            set
            {
                _parseObject.LowercaseName = value;
                RaisePropertyChanged(() => LowercaseName);
            }
        }

        [JsonProperty("description")]
        public string Description
        {
            get { return ParseObject.Description; }
            set
            {
                ParseObject.Description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        [JsonProperty("deliveryNotes")]
        public string DeliveryNotes
        {
            get { return ParseObject.DeliveryNotes; }
            set
            {
                ParseObject.DeliveryNotes = value;
                RaisePropertyChanged(() => DeliveryNotes);
            }
        }

        private string _imageUrl;

        [JsonProperty("imageUrl")]
        public string ImageUrl
        {
            get
            {
                if (_imageUrl == null && _parseObject != null && _parseObject.Image != null &&
                    _parseObject.Image.Url != null)
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
            get { return _parseObject.IsBlocked; }
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
                    ParseObject.Category = value != null
                        ? Parse.ParseObject.CreateWithoutData<ParseStoreCategory>(value)
                        : null;
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
                    _category = new StoreCategory(ParseObject.Category);
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
                    ParseObject.City = value != null ? Parse.ParseObject.CreateWithoutData<ParseCity>(value) : null;
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
                    ParseObject.Country = value != null
                        ? Parse.ParseObject.CreateWithoutData<ParseCountry>(value)
                        : null;
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
                    _country = new Country(ParseObject.Country);
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
                    _city = new City(ParseObject.City);
                return _city;
            }
        }

        [Ignore]
        public IList<string> SearchTokens
        {
            get { return _parseObject != null ? _parseObject.SearchTokens : null; }
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
                    ParseObject.Owner = value != null ? Parse.ParseObject.CreateWithoutData<TojeeroUser>(value) : null;
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
                    _owner = new User(ParseObject.Owner);
                return _owner;
            }
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, bool includeInvisible = false)
        {
            var manager = Mvx.Resolve<IModelEntityManager>();
            var result =
                await
                    manager.Fetch<IProduct, Product>(
                        new StoreProductsQueryLoader(pageSize, offset, manager, this, includeInvisible),
                        Constants.ProductsCacheTimespan.TotalMilliseconds);
            return result;
        }

        public async Task Save()
        {
            if (ParseObject != null)
            {
                await ParseObject.SaveAsync();
                var query =
                    new ParseQuery<ParseStore>().Where(s => s.ObjectId == ParseObject.ObjectId)
                        .Include("category")
                        .Include("country")
                        .Include("city");
                var store = await query.FirstOrDefaultAsync();
                ParseObject = store;
            }
        }

        public async Task SetMainImage(IImage image)
        {
            var imageFile = new ParseFile(image.Name, image.RawImage);
            await imageFile.SaveAsync();
            ParseObject.Image = imageFile;
            ImageUrl = null;
        }

        public async Task FetchCountry()
        {
            using (var writerLock = await _countryLocker.WriterLockAsync())
            {
                if (Country == null)
                    return;
                if (Country.Name != null)
                    return;
                var country = Country as Country;
                if (country != null)
                {
                    await country.ParseObject.FetchAsync();
                    RaisePropertyChanged(() => Country);
                }
            }
        }

        public async Task LoadRelationships()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var query =
                    new ParseQuery<ParseStore>().Where(s => s.ObjectId == ID)
                        .Include("category")
                        .Include("country")
                        .Include("city");
                var store = await query.FirstOrDefaultAsync();
                ParseObject = store;
            }
        }

        #endregion
    }

    [ParseClassName("Store")]
    public class ParseStore : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("lowercase_name")]
        public string LowercaseName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("description")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("deliveryNotes")]
        public string DeliveryNotes
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("image")]
        public ParseFile Image
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("isBlocked")]
        public bool IsBlocked
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("category")]
        public ParseStoreCategory Category
        {
            get { return GetProperty<ParseStoreCategory>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("cityId")]
        public int? CityId
        {
            get { return GetProperty<int?>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("countryId")]
        public int? CountryId
        {
            get { return GetProperty<int?>(); }
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

        [ParseFieldName("owner")]
        public TojeeroUser Owner
        {
            get { return GetProperty<TojeeroUser>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("searchTokens")]
        public IList<string> SearchTokens
        {
            get { return GetProperty<IList<string>>(); }
            set { SetProperty(value); }
        }

        #endregion
    }
}
﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Newtonsoft.Json;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class Product : BaseMultiImagelEntity<ParseProduct>, IProduct
    {
        #region Constructors

        public Product()
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
            get { return base.ParseObject; }
            set
            {
                _imageUrl = null;
                _category = null;
                _subcategory = null;
                _store = null;
                _country = null;
                _city = null;
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

        [JsonProperty("price")]
        public double Price
        {
            get { return _parseObject.Price; }
            set
            {
                _parseObject.Price = value;
                RaisePropertyChanged(() => Price);
                RaisePropertyChanged(() => FormattedPrice);
            }
        }

        [JsonProperty("description")]
        public string Description
        {
            get { return _parseObject.Description; }
            set
            {
                _parseObject.Description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        [Ignore]
        public string FormattedPrice
        {
            get { return Price.ToString("N2"); }
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
                    ParseObject.Category = Parse.ParseObject.CreateWithoutData<ParseProductCategory>(_categoryID);
                }
            }
        }

        private string _subcategoryID;

        [JsonProperty("subcategoryID")]
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
                if (_subcategoryID != value)
                {
                    _subcategoryID = value;
                    _subcategory = null;
                    ParseObject.Subcategory =
                        Parse.ParseObject.CreateWithoutData<ParseProductSubcategory>(_subcategoryID);
                }
            }
        }

        private string _storeID;

        [JsonProperty("storeID")]
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
                if (_storeID != value)
                {
                    _storeID = value;
                    _store = null;
                    ParseObject.Store = Parse.ParseObject.CreateWithoutData<ParseStore>(_storeID);
                }
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
                    ParseObject.City = Parse.ParseObject.CreateWithoutData<ParseCity>(_cityId);
                }
            }
        }

        private string _countryId;

        [JsonProperty("countryID")]
        public string CountryId
        {
            get
            {
                if (_countryId == null && Country != null)
                    _countryId = Country.ID;
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

        [Ignore]
        public ICountry Country
        {
            get
            {
                if (_country == null && ParseObject.Country != null)
                    _country = new Country(ParseObject.Country);
                return _country;
            }
            set
            {
                _countryId = null;
                _country = value;
                RaisePropertyChanged(() => Country);
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
                }
            }
        }

        [Ignore]
        [JsonProperty("_tags")]
        public IList<string> Tags
        {
            get { return _parseObject != null ? _parseObject.Tags : null; }
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
                    _category = new ProductCategory(ParseObject.Category);
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
                    _subcategory = new ProductSubcategory(ParseObject.Subcategory);
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
                    _store = new Store(ParseObject.Store);
                return _store;
            }
        }

        [JsonProperty("status")]
        public ProductStatus Status
        {
            get { return (ProductStatus) _parseObject.Status; }
            set
            {
                _parseObject.Status = (int) value;
                RaisePropertyChanged(() => Status);
            }
        }

        [JsonProperty("notVisible")]
        public bool NotVisible
        {
            get { return _parseObject.NotVisible; }
            set
            {
                _parseObject.NotVisible = value;
                RaisePropertyChanged(() => NotVisible);
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

        public string DisapprovalReason
        {
            get { return _parseObject.DisapprovalReason; }
            set
            {
                _parseObject.DisapprovalReason = value;
                RaisePropertyChanged(() => DisapprovalReason);
            }
        }

        #endregion

        #region Parent

        public override string ToString()
        {
            return ID + " " + Name;
        }

        #endregion

        #region Methods

        public async Task Save()
        {
            if (ParseObject != null)
            {
                await ParseObject.SaveAsync();
                var query =
                    new ParseQuery<ParseProduct>().Where(s => s.ObjectId == ParseObject.ObjectId)
                        .Include("category")
                        .Include("subcategory")
                        .Include("store")
                        .Include("country");
                var product = await query.FirstOrDefaultAsync();
                ParseObject = product;
            }
        }

        public async Task SetMainImage(IImage image)
        {
            var imageFile = new ParseFile(image.Name, image.RawImage);
            await imageFile.SaveAsync();
            ParseObject.Image = imageFile;
            ImageUrl = null;
        }

        public async Task LoadRelationships()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var query =
                    new ParseQuery<ParseProduct>().Where(s => s.ObjectId == ID)
                        .Include("category")
                        .Include("subcategory")
                        .Include("store.owner")
                        .Include("country");
                var product = await query.FirstOrDefaultAsync();
                ParseObject = product;
            }
        }

        #endregion
    }

    [ParseClassName("StoreItem")]
    public class ParseProduct : ParseObject, IParseMultiImageEntity
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

        [ParseFieldName("tags")]
        public IList<string> Tags
        {
            get { return GetProperty<IList<string>>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("price")]
        public double Price
        {
            get { return GetProperty<double>(); }
            set { SetProperty(value); }
        }

        public string FormattedPrice
        {
            get { return Price.ToString("C"); }
        }

        [ParseFieldName("image")]
        public ParseFile Image
        {
            get { return GetProperty<ParseFile>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("images")]
        public ParseRelation<ParseData> Images
        {
            get { return GetRelationProperty<ParseData>(); }
        }

        [ParseFieldName("category")]
        public ParseProductCategory Category
        {
            get { return GetProperty<ParseProductCategory>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("subcategory")]
        public ParseProductSubcategory Subcategory
        {
            get { return GetProperty<ParseProductSubcategory>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("store")]
        public ParseStore Store
        {
            get { return GetProperty<ParseStore>(); }
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

        [ParseFieldName("status")]
        public int Status
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("notVisible")]
        public bool NotVisible
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("isBlocked")]
        public bool IsBlocked
        {
            get { return GetProperty<bool>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("disapprovalReason")]
        public string DisapprovalReason
        {
            get { return GetProperty<string>(); }
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
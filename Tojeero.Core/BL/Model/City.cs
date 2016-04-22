using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class City : BaseLocalizableModelEntity<ParseCity>, ICity
    {
        #region Constructors

        public City()
        {
        }

        public City(ParseCity city = null)
            : base(city)
        {
        }

        #endregion

        #region Properties

        [Ignore]
        public override ParseCity ParseObject
        {
            get { return base.ParseObject; }
            set { base.ParseObject = value; }
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

        [Ignore]
        public string Name
        {
            get
            {
                if (Language == LanguageCode.Arabic && !string.IsNullOrEmpty(Name_ar))
                    return Name_ar;
                return Name_en;
            }
        }

        public string Name_en
        {
            get { return ParseObject.Name_en; }
            set
            {
                ParseObject.Name_en = value;
                RaisePropertyChanged(() => Name);
            }
        }


        public string Name_ar
        {
            get { return ParseObject.Name_ar; }
            set
            {
                ParseObject.Name_ar = value;
                RaisePropertyChanged(() => Name);
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

        #endregion

        #region Parent 

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region implemented abstract members of BaseLocalizableModelEntity

        protected override void raiseCulturalPropertyChange()
        {
            RaisePropertyChanged(() => Name);
        }

        #endregion
    }

    [ParseClassName("City")]
    public class ParseCity : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("country")]
        public ParseCountry Country
        {
            get { return GetProperty<ParseCountry>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("name_en")]
        public string Name_en
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("name_ar")]
        public string Name_ar
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        #endregion
    }
}
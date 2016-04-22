using System.Linq;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Parse;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
{
    public class Country : BaseLocalizableModelEntity<ParseCountry>, ICountry
    {
        #region Private fields and properties

        private ICityManager _cityManager;

        private ICityManager CityManager
        {
            get
            {
                if (_cityManager == null)
                {
                    _cityManager = Mvx.Resolve<ICityManager>();
                }
                return _cityManager;
            }
        }

        #endregion

        #region Constructors

        public Country()
        {
        }

        public Country(ParseCountry country = null)
            : base(country)
        {
        }

        #endregion

        #region Properties

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

        [Ignore]
        public string Currency
        {
            get
            {
                if (Language == LanguageCode.Arabic && !string.IsNullOrEmpty(Currency_ar))
                    return Currency_ar;
                return Currency_en;
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

        public string Currency_en
        {
            get { return ParseObject.Currency_en; }
            set
            {
                ParseObject.Currency_en = value;
                RaisePropertyChanged(() => Currency);
            }
        }

        public string Currency_ar
        {
            get { return ParseObject.Currency_ar; }
            set
            {
                ParseObject.Currency_ar = value;
                RaisePropertyChanged(() => Currency);
            }
        }

        public string CountryPhoneCode
        {
            get { return ParseObject.CountryPhoneCode; }
            set
            {
                ParseObject.CountryPhoneCode = value;
                RaisePropertyChanged(() => CountryPhoneCode);
            }
        }

        private ICity[] _cities;

        [Ignore]
        public ICity[] Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                RaisePropertyChanged(() => Cities);
            }
        }

        #endregion

        #region Public API

        public async Task LoadCities()
        {
            if (Cities != null && Cities.Length > 0)
                return;
            var cities = await CityManager.Fetch(ID);
            if (cities != null)
                Cities = cities.OrderBy(c => c.Name).ToArray();
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
            RaisePropertyChanged(() => Currency);
        }

        #endregion
    }

    [ParseClassName("Country")]
    public class ParseCountry : ParseObject
    {
        #region Constructors

        #endregion

        #region Properties

        [ParseFieldName("name_en")]
        public string Name_en
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("countryId")]
        public int CountryId
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("name_ar")]
        public string Name_ar
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("currency_en")]
        public string Currency_en
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("currency_ar")]
        public string Currency_ar
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("countryPhoneCode")]
        public string CountryPhoneCode
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("cities")]
        public ParseRelation<ParseCity> Cities
        {
            get { return GetRelationProperty<ParseCity>(); }
        }

        #endregion
    }
}
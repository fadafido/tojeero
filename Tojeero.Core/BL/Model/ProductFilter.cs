using System.Collections.Generic;
using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Services.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.Model
{
    public class ProductFilter : MvxViewModel, IProductFilter
    {
        #region Private fields

        private readonly IAuthenticationService _authService;
        private readonly ICountryManager _countryManager;
        private readonly ICityManager _cityManager;

        #endregion

        #region Constructors

        public ProductFilter()
        {
            _authService = Mvx.Resolve<IAuthenticationService>();
            _countryManager = Mvx.Resolve<ICountryManager>();
            _cityManager = Mvx.Resolve<ICityManager>();

            initialize();
        }

        #endregion

        #region IProductFilter implementation

        private IProductCategory _category;

        public IProductCategory Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged(() => Category);
            }
        }

        private IProductSubcategory _subcategory;

        public IProductSubcategory Subcategory
        {
            get { return _subcategory; }
            set
            {
                _subcategory = value;
                RaisePropertyChanged(() => Subcategory);
            }
        }


        private ICountry _country;

        public ICountry Country
        {
            get { return _country; }
            set
            {
                _country = value;
                RaisePropertyChanged(() => Country);
            }
        }

        private ICity _city;

        public ICity City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged(() => City);
            }
        }


        private double? _startPrice;

        public double? StartPrice
        {
            get { return _startPrice; }
            set
            {
                _startPrice = value;
                RaisePropertyChanged(() => StartPrice);
            }
        }

        private double? _endPrice;

        public double? EndPrice
        {
            get { return _endPrice; }
            set
            {
                _endPrice = value;
                RaisePropertyChanged(() => EndPrice);
            }
        }

        private ObservableCollection<string> _tags = new ObservableCollection<string>();

        public ObservableCollection<string> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        public IProductFilter Clone()
        {
            var filter = new ProductFilter
            {
                Category = Category,
                Subcategory = Subcategory,
                Country = Country,
                City = City,
                StartPrice = StartPrice,
                EndPrice = EndPrice
            };
            var tags = new ObservableCollection<string>();
            tags.AddRange(Tags);
            filter.Tags = tags;
            return filter;
        }

        public void SetCountryID(string countryId)
        {
            if (!string.IsNullOrEmpty(countryId))
            {
                var country = _countryManager.Create();
                country.ID = countryId;
                Country = country;
            }
            else
            {
                Country = null;
            }
        }

        public void SetCityID(string cityId)
        {
            if (!string.IsNullOrEmpty(cityId))
            {
                var city = _cityManager.Create();
                city.ID = cityId;
                City = city;
            }
            else
            {
                City = null;
            }
        }

        #endregion

        #region Overriding Parent

        public override string ToString()
        {
            var components = new List<string>();
            if (Category != null)
            {
                components.Add("ct:" + Category.ID);
            }

            if (Subcategory != null)
            {
                components.Add("sct:" + Subcategory.ID);
            }

            if (Country != null)
            {
                components.Add("cn:" + Country.ID);
            }

            if (City != null)
            {
                components.Add("cty:" + City.ID);
            }

            if (Tags != null && Tags.Count > 0)
            {
                components.Add("t:" + string.Join(",", Tags.SubCollection(0, Constants.ParseContainsAllLimit)));
            }

            if (StartPrice != null)
            {
                components.Add("sp:" + StartPrice.Value);
            }

            if (EndPrice != null)
            {
                components.Add("ep:" + EndPrice.Value);
            }
            return string.Join("|", components);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var filter = obj as IProductFilter;
            if (filter == null)
                return false;

            if (!(Category == null && filter.Category == null ||
                  Category != null && filter.Category != null && Category.ID == filter.Category.ID))
                return false;

            if (!(Subcategory == null && filter.Subcategory == null ||
                  Subcategory != null && filter.Subcategory != null && Subcategory.ID == filter.Subcategory.ID))
                return false;

            if (!(Country == null && filter.Country == null ||
                  Country != null && filter.Country != null && Country.ID == filter.Country.ID))
                return false;

            if (!(City == null && filter.City == null ||
                  City != null && filter.City != null && City.ID == filter.City.ID))
                return false;

            if (!(StartPrice == null && filter.StartPrice == null ||
                  StartPrice != null && filter.StartPrice != null && StartPrice.Value == filter.StartPrice.Value))
                return false;

            if (!(EndPrice == null && filter.EndPrice == null ||
                  EndPrice != null && filter.EndPrice != null && EndPrice.Value == filter.EndPrice.Value))
                return false;

            if (Tags == null && filter.Tags == null)
            {
                return true;
            }
            if (Tags != null && filter.Tags != null)
            {
                if (Tags.Count != filter.Tags.Count)
                    return false;
                for (var i = 0; i < Tags.Count; i++)
                {
                    if (Tags[i] != filter.Tags[i])
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(ProductFilter a, ProductFilter b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ProductFilter a, ProductFilter b)
        {
            return !(a == b);
        }

        #endregion

        #region Utility methods

        private void initialize()
        {
            string countryId = null;
            if (_authService.CurrentUser != null)
            {
                countryId = _authService.CurrentUser?.CountryId;
            }
            countryId = !string.IsNullOrEmpty(countryId) ? countryId : Settings.CountryId;

            SetCountryID(countryId);
        }

        #endregion
    }
}
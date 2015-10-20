using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Tojeero.Core.Services;
using Tojeero.Core.Messages;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
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
			:base()
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
			get
			{
				return _category; 
			}
			set
			{
				_category = value; 
				RaisePropertyChanged(() => Category); 
			}
		}

		private IProductSubcategory _subcategory;

		public IProductSubcategory Subcategory
		{ 
			get
			{
				return _subcategory; 
			}
			set
			{
				_subcategory = value; 
				RaisePropertyChanged(() => Subcategory); 
			}
		}


		private ICountry _country;
		public ICountry Country
		{ 
			get
			{
				return _country; 
			}
			set
			{
				_country = value; 
				RaisePropertyChanged(() => Country); 
			}
		}

		private ICity _city;

		public ICity City
		{ 
			get
			{
				return _city; 
			}
			set
			{
				_city = value; 
				RaisePropertyChanged(() => City); 
			}
		}
	

		private double? _startPrice = 100;

		public double? StartPrice
		{ 
			get
			{
				return _startPrice; 
			}
			set
			{
				_startPrice = value; 
				RaisePropertyChanged(() => StartPrice); 
			}
		}

		private double? _endPrice = 1000;

		public double? EndPrice
		{ 
			get
			{
				return _endPrice; 
			}
			set
			{
				_endPrice = value; 
				RaisePropertyChanged(() => EndPrice); 
			}
		}

		private ObservableCollection<string> _tags = new ObservableCollection<string>();
		public ObservableCollection<string> Tags
		{ 
			get
			{
				return _tags; 
			}
			set
			{
				_tags = value; 
				RaisePropertyChanged(() => Tags); 
			}
		}

		public IProductFilter Clone()
		{
			var filter = new ProductFilter(){
				Category = this.Category,
				Subcategory = this.Subcategory,
				Country = this.Country,
				City = this.City,
				StartPrice = this.StartPrice,
				EndPrice = this.EndPrice
			};
			var tags = new ObservableCollection<string>();
			tags.AddRange(this.Tags);
			filter.Tags = tags;
			return filter;
		}

		#endregion

		#region Overriding Parent

		public override string ToString()
		{
			List<string> components = new List<string>();
			if (this.Category != null)
			{
				components.Add("ct:" + this.Category.ID);
			}

			if (this.Subcategory != null)
			{
				components.Add("sct:" + this.Subcategory.ID);
			}

			if (this.Country != null)
			{
				components.Add("cn:" + this.Country.CountryId);
			}

			if (this.City != null)
			{
				components.Add("cty:" + this.City.CityId);
			}

			if (this.Tags != null && this.Tags.Count > 0)
			{
				components.Add("t:" + string.Join(",",this.Tags.SubCollection(0, Constants.ParseContainsAllLimit)));
			}

			if (this.StartPrice != null)
			{
				components.Add("sp:" + this.StartPrice.Value);
			}

			if (this.EndPrice != null)
			{
				components.Add("ep:" + this.EndPrice.Value);
			}
			return string.Join("|", components);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (System.Object.ReferenceEquals(this, obj))
				return true;

			var filter = obj as IProductFilter;
			if (filter == null)
				return false;

			if (!(this.Category == null && filter.Category == null ||
			   this.Category != null && filter.Category != null && this.Category.ID == filter.Category.ID))
				return false;
			
			if (!(this.Subcategory == null && filter.Subcategory == null ||
				this.Subcategory != null && filter.Subcategory != null && this.Subcategory.ID == filter.Subcategory.ID))
				return false;	
			
			if (!(this.City == null && filter.City == null ||
				this.City != null && filter.City != null && this.City.CityId == filter.City.CityId))
				return false;	
			
			if (!(this.StartPrice == null && filter.StartPrice == null ||
				this.StartPrice != null && filter.StartPrice != null && this.StartPrice.Value == filter.StartPrice.Value))
				return false;
			
			if (!(this.StartPrice == null && filter.StartPrice == null ||
				this.StartPrice != null && filter.StartPrice != null && this.StartPrice.Value == filter.StartPrice.Value))
				return false;	

			if (!(this.EndPrice == null && filter.EndPrice == null ||
				this.EndPrice != null && filter.EndPrice != null && this.EndPrice.Value == filter.EndPrice.Value))
				return false;	
			
			if (this.Tags == null && filter.Tags == null)
			{
				return true;
			}
			else if (this.Tags != null && filter.Tags != null)
			{
				if (this.Tags.Count != filter.Tags.Count)
					return false;
				for (int i = 0; i < this.Tags.Count; i++)
				{
					if (this.Tags[i] != filter.Tags[i])
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
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
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
			if (_authService.CurrentUser != null)
			{
				
				var countryId = _authService.CurrentUser != null ? _authService.CurrentUser.CountryId : Settings.CountryId;
				if (countryId != null)
				{
					var country = _countryManager.Create();
					country.CountryId = countryId.Value;
					this._country = country;
				}
					
				var cityId = _authService.CurrentUser != null ? _authService.CurrentUser.CityId : Settings.CityId;
				if (cityId != null)
				{
					var city = _cityManager.Create();
					city.CityId = cityId.Value;
					this._city = city;
				}
			}
		}

		#endregion

	}
}
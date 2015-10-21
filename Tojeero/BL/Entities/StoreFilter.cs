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

	public class StoreFilter : MvxViewModel, IStoreFilter
	{
		#region Private fields

		private readonly IAuthenticationService _authService;
		private readonly ICountryManager _countryManager;
		private readonly ICityManager _cityManager;

		#endregion

		#region Constructors

		public StoreFilter()
			:base()
		{
			_authService = Mvx.Resolve<IAuthenticationService>();
			_countryManager = Mvx.Resolve<ICountryManager>();
			_cityManager = Mvx.Resolve<ICityManager>();

			initialize();
		}

		#endregion

		#region IStoreFilter implementation

		private IStoreCategory _category;

		public IStoreCategory Category
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

		public IStoreFilter Clone()
		{
			var filter = new StoreFilter(){
				Category = this.Category,
				Country = this.Country,
				City = this.City
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
				
			return string.Join("|", components);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (System.Object.ReferenceEquals(this, obj))
				return true;

			var filter = obj as IStoreFilter;
			if (filter == null)
				return false;

			if (!(this.Category == null && filter.Category == null ||
			   this.Category != null && filter.Category != null && this.Category.ID == filter.Category.ID))
				return false;
			
			if (!(this.Country == null && filter.Country == null ||
				this.Country != null && filter.Country != null && this.Country.CountryId == filter.Country.CountryId))
				return false;
			
			if (!(this.City == null && filter.City == null ||
				this.City != null && filter.City != null && this.City.CityId == filter.City.CityId))
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

		public static bool operator ==(StoreFilter a, StoreFilter b)
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

		public static bool operator !=(StoreFilter a, StoreFilter b)
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

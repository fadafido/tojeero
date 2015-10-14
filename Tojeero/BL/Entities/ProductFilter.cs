using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.ObjectModel;
using Cirrious.CrossCore;
using Tojeero.Core.Services;
using Tojeero.Core.Messages;
using Cirrious.MvvmCross.Plugins.Messenger;

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
	

		private double? _startPrice;

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

		private double? _endPrice;

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

		private ObservableCollection<string> _tags;

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

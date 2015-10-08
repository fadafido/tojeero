using System;
using System.Linq;
using Parse;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Newtonsoft.Json;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using System.Threading.Tasks;

namespace Tojeero.Core
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
			:base()
		{
			
		}

		public Country(ParseCountry country = null)
			: base(country)
		{
			
		}


		#endregion

		#region Properties

		[Ignore]
		public override ParseCountry ParseObject
		{
			get
			{
				return base.ParseObject;
			}
			set
			{
				
				base.ParseObject = value;
			}
		}

		public int CountryId
		{
			get
			{
				return this.ParseObject.CountryId;
			}
			set
			{
				if (this.ParseObject.CountryId != value)
				{
					this.ParseObject.CountryId = value;
					this.RaisePropertyChanged(() => CountryId);
					this.Cities = null;
				}
			}
		}

		[Ignore]
		public string Name
		{
			get
			{
				switch (this.Language)
				{
					case LanguageCode.Arabic:
						return Name_ar;
						break;
					default:
						return Name_en;	
						break;
				}
			}
		}

		[Ignore]
		public string Currency
		{
			get
			{
				switch (this.Language)
				{
					case LanguageCode.Arabic:
						return Currency_ar;
						break;
					default:
						return Currency_en;	
						break;
				}
			}
		}

		public string Name_en
		{
			get
			{
				return this.ParseObject.Name_en;
			}
			set
			{
				this.ParseObject.Name_en = value;
				this.RaisePropertyChanged(() => Name);
			}
		}


		public string Name_ar
		{
			get
			{
				return this.ParseObject.Name_ar;
			}
			set
			{
				this.ParseObject.Name_ar = value;
				this.RaisePropertyChanged(() => Name);
			}
		}

		public string Currency_en
		{
			get
			{
				return this.ParseObject.Currency_en;
			}
			set
			{
				this.ParseObject.Currency_en = value;
				this.RaisePropertyChanged(() => Currency);
			}
		}

		public string Currency_ar
		{
			get
			{
				return this.ParseObject.Currency_ar;
			}
			set
			{
				this.ParseObject.Currency_ar = value;
				this.RaisePropertyChanged(() => Currency);
			}
		}

		public string CountryPhoneCode
		{
			get
			{
				return ParseObject.CountryPhoneCode;
			}
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
			get
			{
				return _cities; 
			}
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
			if (this.Cities != null && this.Cities.Length > 0)
				return;
			var cities = await CityManager.FetchCities(this.CountryId);
			this.Cities = cities.OrderBy(c => c.Name).ToArray();
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

		public ParseCountry()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("name_en")]
		public string Name_en
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("countryId")]
		public int CountryId
		{
			get
			{
				return GetProperty<int>();
			}
			set
			{
				SetProperty<int>(value);
			}
		}

		[ParseFieldName("name_ar")]
		public string Name_ar
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("currency_en")]
		public string Currency_en
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("currency_ar")]
		public string Currency_ar
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		[ParseFieldName("countryPhoneCode")]
		public string CountryPhoneCode
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
			}
		}

		#endregion
	}
}


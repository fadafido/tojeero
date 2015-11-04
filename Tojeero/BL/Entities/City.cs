using System;
using Parse;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Newtonsoft.Json;
using Tojeero.Core.Services;
using Cirrious.CrossCore;

namespace Tojeero.Core
{
	public class City : BaseLocalizableModelEntity<ParseCity>, ICity
	{
		#region Constructors

		public City()
			:base()
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
			get
			{
				return base.ParseObject;
			}
			set
			{

				base.ParseObject = value;
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
					this.ParseObject.Country = Parse.ParseObject.CreateWithoutData<ParseCountry>(_countryId);
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
			
		private ICountry _country;
		[Ignore]
		public ICountry Country
		{ 
			get
			{
				if (_country == null)
					_country = new Country(this.ParseObject.Country);
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

		public ParseCity()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("country")]
		public ParseCountry Country
		{
			get
			{
				return GetProperty<ParseCountry>();
			}
			set
			{
				SetProperty<ParseCountry>(value);
			}
		}

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

		#endregion
	}
}


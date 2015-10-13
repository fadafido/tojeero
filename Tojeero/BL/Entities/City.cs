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

		public int CityId
		{
			get
			{
				return this.ParseObject.CityId;
			}
			set
			{
				this.ParseObject.CityId = value;
				this.RaisePropertyChanged(() => CityId);
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
				this.ParseObject.CountryId = value;
				this.RaisePropertyChanged(() => CountryId);
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

		[ParseFieldName("cityId")]
		public int CityId
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


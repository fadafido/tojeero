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
	public class ProductCategory : BaseLocalizableModelEntity<ParseProductCategory>, IProductCategory
	{
		#region Constructors

		public ProductCategory()
			:base()
		{

		}

		public ProductCategory(ParseProductCategory category = null)
			: base(category)
		{

		}


		#endregion

		#region Properties

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

		public string Name_en
		{
			get
			{
				return this.ParseObject.Name_en;
			}
			set
			{
				this.ParseObject.Name_en = value;
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

	[ParseClassName("ProductCategory")]
	public class ParseProductCategory : ParseObject
	{
		#region Constructors

		public ParseProductCategory()
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


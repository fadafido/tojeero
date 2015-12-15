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
	public class ProductSubcategory : BaseLocalizableModelEntity<ParseProductSubcategory>, IProductSubcategory
	{
		#region Constructors

		public ProductSubcategory()
			:base()
		{

		}

		public ProductSubcategory(ParseProductSubcategory subcategory = null)
			: base(subcategory)
		{

		}


		#endregion

		#region Properties

		private string _categoryID;
		public string CategoryID
		{ 
			get
			{
				if (_categoryID == null && _parseObject != null && _parseObject.Category != null)
					_categoryID = _parseObject.Category.ObjectId;
				return _categoryID; 
			}
			set
			{
				_categoryID = value; 
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

	[ParseClassName("ProductSubcategory")]
	public class ParseProductSubcategory : ParseObject
	{
		#region Constructors

		public ParseProductSubcategory()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("category")]
		public ParseProductCategory Category
		{
			get
			{
				return GetProperty<ParseProductCategory>();
			}
			set
			{
				SetProperty<ParseProductCategory>(value);
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


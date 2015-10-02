using System;
using Parse;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	[ParseClassName("StoreItem")]
	public class Product : BaseModelEntity, IProduct
	{
		#region Constructors

		public Product()
		{
		}

		#endregion

		#region Properties

		[ParseFieldName("name")]
		public string Name
		{
			get
			{
				return GetProperty<string>();
			}
			set
			{
				SetProperty<string>(value);
				RaisePropertyChanged(() => Name);
			}
		}

		[ParseFieldName("price")]
		public double Price
		{
			get
			{
				return GetProperty<double>();
			}
			set
			{
				SetProperty<double>(value);
				RaisePropertyChanged(() => Price);
				RaisePropertyChanged(() => FormattedPrice);
			}
		}

		public string FormattedPrice
		{
			get
			{
				return Price.ToString("C");
			}
		}
			
		public string ImageUrl
		{
			get
			{
				return Image != null ? Image.Url.ToString() : null;
			}
		}

		[ParseFieldName("image")]
		[Ignore]
		public ParseFile Image
		{
			get
			{
				return GetProperty<ParseFile>();
			}
			set
			{
				SetProperty<ParseFile>(value);
				RaisePropertyChanged(() => ImageUrl);
			}
		}
		#endregion
	}
}


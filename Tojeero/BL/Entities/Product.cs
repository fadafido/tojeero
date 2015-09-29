﻿using System;
using Parse;

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
			}
		}
			
		public Uri ImageUrl
		{
			get
			{
				return Image != null ? Image.Url : null;
			}
		}

		[ParseFieldName("image")]
		public ParseFile Image
		{
			get
			{
				return GetProperty<ParseFile>();
			}
			set
			{
				SetProperty<ParseFile>(value);
			}
		}
		#endregion
	}
}


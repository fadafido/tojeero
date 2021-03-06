﻿using System;
using Parse;

namespace Tojeero.Core
{
	public static class ParseInitialize
	{
		public static void Initialize()
		{
			//Register custom subclasses
			ParseObject.RegisterSubclass<TojeeroUser>();
			ParseObject.RegisterSubclass<ParseProduct>();
			ParseObject.RegisterSubclass<ParseStore>();
			ParseObject.RegisterSubclass<ParseCountry>();
			ParseObject.RegisterSubclass<ParseCity>();
			ParseObject.RegisterSubclass<ParseProductCategory>();
			ParseObject.RegisterSubclass<ParseProductSubcategory>();
			ParseObject.RegisterSubclass<ParseStoreCategory>();
			ParseObject.RegisterSubclass<ParseTag>();
			ParseObject.RegisterSubclass<ReservedName>();
			ParseObject.RegisterSubclass<ParseData>();

			//Initialize parse
			ParseClient.Initialize(Constants.ParseApplicationId, Constants.ParseDotNetKey);
			ParseFacebookUtils.Initialize(Constants.FacebookAppId);
		}
	}
}


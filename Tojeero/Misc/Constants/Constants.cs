﻿using System;

namespace Tojeero.Core
{
	public static class Constants
	{
		/*****************SOCIAL INTEGRATION*****************/
		public static string FacebookAppId = "1494452237545846";
		public static string FacebookAuthorizationUrl = "https://m.facebook.com/dialog/oauth/";
		public static string FacebookRedirectUrl = "http://www.facebook.com/connect/login_success.html"; 
		public const int FBProfilePicSize = 300;

		/*****************PARSE.COM*****************/
		public static string ParseApplicationId = "1gNtcAK31ETKoYCeDAKpaBfjh82FQQR3etWWVTHa";
		public static string ParseDotNetKey = "qcdEV2teifcnFbmeLIBPMlUkcz0WgGPAJ9kAvGD2";

		/*****************MESSAGES*****************/
		public static string SessionStateChangedMessage = "com.tojeero.tojeero:SessionStateChangedMessage";

		/*****************TIMEOUTS*****************/
		public static int DefaultTimeout = 20000;
		public static int FetchProductsTimeout = 10000;
		public static int FetchStoresTimeout = 10000;
		public static int FindProductsTimeout = 10000;
		public static int FindStoresTimeout = 10000;
		public static int FetchCountriesTimeout = 10000;
		public static int FetchCitiesTimeout = 10000;
		public static int FetchProductSubcategoriesTimeout = 10000;
		public static int FetchStoreSubcategoriesTimeout = 10000;
		public static int FetchTagsTimeout = 10000;
		public static int FindTagsTimeout = 10000;
		public static int SearchTimeout = 1000;
		public static int SaveStoreTimeout = 10000;

		/*****************PAGINATION*****************/
		public static int ProductsPageSize = 50;
		public static int StoresPageSize = 50;
		public static int TagsPageSize = 50;

		/*****************MISC*****************/
		public static string XamarinInsightsApiKey = "641ba3e3bf2f2764d06bc254a896aed9c8175a94";
		public static int ParseContainsAllLimit = 9;

		/*****************Cache timespans*****************/
		public static TimeSpan ImageCacheTimespan = TimeSpan.FromDays(1);
		public static TimeSpan ProductsCacheTimespan = TimeSpan.FromMinutes(5);
		public static TimeSpan StoresCacheTimespan = TimeSpan.FromMinutes(5);
		public static TimeSpan TagsCacheTimespan = TimeSpan.FromMinutes(5);

		/*****************DATABASE*****************/
		public static string DatabaseFileName = "Tojeero.sqlite";
	}
}


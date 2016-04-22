using System;

namespace Tojeero.Core
{
    public static class Constants
    {
        /*****************SOCIAL INTEGRATION*****************/
        public static string FacebookAppId = "1494452237545846";
        public static string FacebookAuthorizationUrl = "https://m.facebook.com/dialog/oauth/";
        public static string FacebookRedirectUrl = "http://www.facebook.com/connect/login_success.html";
        public const int FBProfilePicSize = 300;

        /*****************BACKEND KEYS*****************/
        public static string ParseApplicationId = "1gNtcAK31ETKoYCeDAKpaBfjh82FQQR3etWWVTHa";
        public static string ParseDotNetKey = "qcdEV2teifcnFbmeLIBPMlUkcz0WgGPAJ9kAvGD2";
        public static string AlgoliaApplicationId = "I72QVLIB9D";
        public static string AlgoliaSecurityKey = "92e4f8a9553b57b20d4d50f37d3232dc";
        public static string BackendRequestKey = "AN524MNHR";
        public static string PubNubPublishKey = "pub-c-3fa3a8a7-66a0-4b02-b74d-4de8fe2b050a";
        public static string PubNubSubscribeKey = "sub-c-5ec2cc14-c43e-11e5-a9aa-02ee2ddab7fe";
        public static int QuickbloxAppId = 36377;
        public static string QuickbloxAuthKey = "m3O5VaHpSh9xE3D";
        public static string QuickbloxAuthSecret = "xy-CN9zbFn6kddS";

        /*****************MESSAGES*****************/
        public static string SessionStateChangedMessage = "com.tojeero.tojeero:SessionStateChangedMessage";

        /*****************TIMEOUTS*****************/
        public static int DefaultTimeout = 20000;
        public static int FetchProductsTimeout = 20000;
        public static int FetchStoresTimeout = 20000;
        public static int FindProductsTimeout = 20000;
        public static int FindStoresTimeout = 20000;
        public static int FetchCountriesTimeout = 20000;
        public static int FetchCitiesTimeout = 20000;
        public static int FetchProductSubcategoriesTimeout = 20000;
        public static int FetchStoreSubcategoriesTimeout = 20000;
        public static int FetchTagsTimeout = 10000;
        public static int FindTagsTimeout = 10000;
        public static int SearchTimeout = 20000;
        public static int SaveStoreTimeout = 20000;
        public static int SaveProductTimeout = 20000;

        /*****************PAGINATION*****************/
        public static int ProductsPageSize = 50;
        public static int StoresPageSize = 50;
        public static int TagsPageSize = 50;
        public static int ChatChannelsPageSize = 50;

        /*****************MISC*****************/
        public static string XamarinInsightsApiKey = "641ba3e3bf2f2764d06bc254a896aed9c8175a94";
        public static int ParseContainsAllLimit = 9;
        public static int MaxPixelDimensionOfImages = 500;

        /*****************Cache timespans*****************/
        public static TimeSpan ImageCacheTimespan = TimeSpan.FromDays(1);
        public static TimeSpan ProductsCacheTimespan = TimeSpan.FromMinutes(5);
        public static TimeSpan StoresCacheTimespan = TimeSpan.FromMinutes(5);
        public static TimeSpan TagsCacheTimespan = TimeSpan.FromMinutes(5);

        /*****************DATABASE*****************/
        public static string DatabaseFileName = "Tojeero.sqlite";

        /*****************ALGOLIA*****************/
        public static string ProductIndex = "Products";
        public static string StoreIndex = "Stores";
    }
}
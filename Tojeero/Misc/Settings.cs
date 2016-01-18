using System.Collections.Generic;
using Cirrious.CrossCore;
using Tojeero.Core.Services;
using Tojeero.Core.Toolbox;
using Refractored.Xam.Settings.Abstractions;
using Refractored.Xam.Settings;

namespace Tojeero.Core
{
	public static class Settings
	{
		private static ISettings AppSettings
		{
			get
			{
				return CrossSettings.Current;
			}
		}

		#region User Settings

		#region Product list mode

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string ProductListModeKey = "ProductListMode";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly ListMode ProductListModeDefault = ListMode.Large;

		/// <summary>
		/// Gets or sets the ProductListMode value
		/// </summary>
		public static ListMode ProductListMode
		{
			get
			{
				return AppSettings.GetValueOrDefault(ProductListModeKey, ProductListModeDefault);
			}
			set
			{
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(ProductListModeKey, value))
					AppSettings.Save();
			}
		}

		#endregion

		#region Current user

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string CurrentUserKey = "CurrentUser";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly string CurrentUserDefault = null;

		/// <summary>
		/// Gets or sets the CurrentUser value
		/// </summary>
		public static string CurrentUser
		{
			get
			{
				return AppSettings.GetValueOrDefault(CurrentUserKey, CurrentUserDefault);
			}
			set
			{
				if (value == null)
					AppSettings.Remove(CurrentUserKey);
				else
					AppSettings.AddOrUpdateValue(CurrentUserKey, value);
			}
		}

		#endregion

		#region Analytics
		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string IsAnalyticsPermittedByUserKey = "IsAnalyticsPermittedByUser";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly bool IsAnalyticsPermittedByUserDefault = true;

		/// <summary>
		/// Gets or sets the IsAnalyticsPermittedByUser value
		/// </summary>
		public static bool IsAnalyticsPermittedByUser
		{
			get
			{
				return AppSettings.GetValueOrDefault(IsAnalyticsPermittedByUserKey, IsAnalyticsPermittedByUserDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(IsAnalyticsPermittedByUserKey, value);
			}
		}
		#endregion

		#region Country ID

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string CountryIdKey = "CountryId";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly string CountryIdDefault = null;

		/// <summary>
		/// Gets or sets the CountryId value
		/// </summary>
		public static string CountryId
		{
			get
			{
				return AppSettings.GetValueOrDefault(CountryIdKey, CountryIdDefault);
			}
			set
			{
				if (value == null)
					AppSettings.Remove(CountryIdKey);
				else
					AppSettings.AddOrUpdateValue(CountryIdKey, value);
			}
		}

		#endregion

		#region City ID

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string CityIdKey = "CityId";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly string CityIdDefault = null;

		/// <summary>
		/// Gets or sets the CityId value
		/// </summary>
		public static string CityId
		{
			get
			{
				return AppSettings.GetValueOrDefault(CityIdKey, CityIdDefault);
			}
			set
			{
				
				if (value == null)
					AppSettings.Remove(CityIdKey);
				else
					AppSettings.AddOrUpdateValue(CityIdKey, value);
			}
		}

		#endregion

		#region Language

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string LanguageKey = "Language";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly LanguageCode LanguageDefault = LanguageCode.Unknown;

		/// <summary>
		/// Gets or sets the Language value
		/// </summary>
		public static LanguageCode Language
		{
			get
			{
				return AppSettings.GetValueOrDefault(LanguageKey, LanguageDefault);
			}
			set
			{
				if (value == null)
					AppSettings.Remove(LanguageKey);
				else
					AppSettings.AddOrUpdateValue(LanguageKey, value);
			}
		}

		#endregion

		#endregion

		#region Internal settings

		#region Database Schema Version

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string DatabaseSchemaVersionKey = "DatabaseSchemaVersion";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly int DatabaseSchemaVersionDefault = 0;

		/// <summary>
		/// Gets or sets the DatabaseSchemaVersion value
		/// </summary>
		public static int DatabaseSchemaVersion
		{
			get
			{
				return AppSettings.GetValueOrDefault(DatabaseSchemaVersionKey, DatabaseSchemaVersionDefault);
			}
			set
			{
				AppSettings.AddOrUpdateValue(DatabaseSchemaVersionKey, value);
			}
		}

		#endregion

		#region IsUserPreferancesSet

		/// <summary>
		/// Key for your setting
		/// </summary>
		public const string IsUserPreferancesSetKey = "IsUserPreferancesSet";
		/// <summary>
		/// default value for your setting
		/// </summary>
		public static readonly bool IsUserPreferancesSetDefault = false;

		/// <summary>
		/// Gets or sets the IsUserPreferancesSet value
		/// </summary>
		public static bool IsUserPreferancesSet
		{
			get
			{
				return AppSettings.GetValueOrDefault(IsUserPreferancesSetKey, IsUserPreferancesSetDefault);
			}
			set
			{
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(IsUserPreferancesSetKey, value))
					AppSettings.Save();
			}
		}

		#endregion
			
		#endregion
	}
}

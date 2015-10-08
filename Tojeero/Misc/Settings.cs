using System.Collections.Generic;
using Cirrious.CrossCore;
using Refractored.MvxPlugins.Settings;
using Tojeero.Core.Services;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
	public static class Settings
	{
		private static ISettings _settings;
		private static ISettings AppSettings
		{
			get
			{
				return _settings ?? (_settings = Mvx.GetSingleton<ISettings>());
			}
		}

		#region User Settings

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
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(CurrentUserKey, value))
					AppSettings.Save();
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
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(IsAnalyticsPermittedByUserKey, value))
					AppSettings.Save();
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
		public static readonly int? CountryIdDefault = null;

		/// <summary>
		/// Gets or sets the CountryId value
		/// </summary>
		public static int? CountryId
		{
			get
			{
				return AppSettings.GetValueOrDefault(CountryIdKey, CountryIdDefault);
			}
			set
			{
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(CountryIdKey, value))
					AppSettings.Save();
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
		public static readonly int? CityIdDefault = null;

		/// <summary>
		/// Gets or sets the CityId value
		/// </summary>
		public static int? CityId
		{
			get
			{
				return AppSettings.GetValueOrDefault(CityIdKey, CityIdDefault);
			}
			set
			{
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(CityIdKey, value))
					AppSettings.Save();
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
		public static readonly LanguageCode? LanguageDefault = null;

		/// <summary>
		/// Gets or sets the Language value
		/// </summary>
		public static LanguageCode? Language
		{
			get
			{
				return AppSettings.GetValueOrDefault(LanguageKey, LanguageDefault);
			}
			set
			{
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(LanguageKey, value))
					AppSettings.Save();
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
				//if value has changed then save it!
				if (AppSettings.AddOrUpdateValue(DatabaseSchemaVersionKey, value))
					AppSettings.Save();
			}
		}

		#endregion
			
		#endregion
	}
}

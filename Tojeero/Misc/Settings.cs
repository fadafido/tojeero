using System.Collections.Generic;
using Cirrious.CrossCore;
using Refractored.MvxPlugins.Settings;

namespace Tojeero.Core
{
	/// <summary>
	/// This is the Settings static class that can be used in your Core solution or in any
	/// of your client applications. All settings are laid out the same exact way with getters
	/// and setters. 
	/// </summary>
	public static class Settings
	{
		/// <summary>
		/// Simply setup your settings once when it is initialized.
		/// </summary>
		private static ISettings _settings;
		private static ISettings AppSettings
		{
			get
			{
				return _settings ?? (_settings = Mvx.GetSingleton<ISettings>());
			}
		}

		#region User Settings
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

		#region Internal settings
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
	}
}

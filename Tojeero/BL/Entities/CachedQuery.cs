using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Reflection;

namespace Tojeero.Core
{
	public class CachedQuery : ICachedQuery
	{
		#region Constructors

		public CachedQuery()
		{
			
		}

		#endregion

		#region Static API

		public static string GetEntityCacheName<T>()
		{
			return GetEntityCacheName(typeof(T));
		}

		public static string GetEntityCacheName(Type t)
		{
			if (t == null)
				return null;
			var name = t.GetTypeInfo().Name;
			return name;
		}

		#endregion

		#region ICachedQuery implementation

		[PrimaryKey]
		public string ID { get; set; }
		public string EntityName { get; set; }
		public DateTime? LastFetchedAt { get; set; }
		public double? ExpiresIn { get; set; }

		#endregion

		#region Public properties

		[Ignore]
		public bool IsExpired
		{
			get
			{
				if (LastFetchedAt == null || ExpiresIn == null)
					return false;
				var now = DateTime.UtcNow;
				var expires = LastFetchedAt.Value.Add(TimeSpan.FromMilliseconds(ExpiresIn.Value));
				return now >= expires;
			}
		}

		#endregion
	}
}


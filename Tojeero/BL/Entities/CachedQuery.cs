using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	public class CachedQuery : ICachedQuery
	{
		#region Constructors

		public CachedQuery()
		{
		}

		#endregion

		#region ICachedQuery implementation

		[PrimaryKey]
		public string ID { get; set; }
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
				return DateTime.UtcNow <= LastFetchedAt.Value.Add(TimeSpan.FromSeconds(ExpiresIn.Value));
			}
		}

		#endregion
	}
}


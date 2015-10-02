using System;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	public interface ICachedQuery : IUniqueEntity
	{
		/// <summary>
		/// Gets or sets the date when the query has been fetched last time in UTC
		/// </summary>
		/// <value>The last fetched date in UTC.</value>
		DateTime? LastFetchedAt { get; set; }
		/// <summary>
		/// Gets or sets the miliseconds in which the cached query should be considered as expired after last time fetched.
		/// </summary>
		/// <value>Miliseconds.</value>
		double? ExpiresIn { get; set; }
	}
}


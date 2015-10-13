using System;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;

namespace Tojeero.Core
{
	public class SearchToken : ISearchToken
	{
		public SearchToken()
		{}

		[PrimaryKey]
		public string ID { get; set; }
		public string EntityID { get; set; }
		public string EntityType { get; set; }
		public string Token { get; set; }
	}
}


using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.Model
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


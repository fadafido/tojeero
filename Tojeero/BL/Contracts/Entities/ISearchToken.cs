using System;

namespace Tojeero.Core
{
	public interface ISearchToken : IUniqueEntity
	{
		string EntityID { get; set; }
		string EntityType { get; set; }
		string Token { get; set; }
	}
}


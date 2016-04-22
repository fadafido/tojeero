using System.Collections.Generic;

namespace Tojeero.Core.Model.Contracts
{
	public interface ISearchableEntity : IUniqueEntity
	{
		IList<string> SearchTokens { get; set; }
	}
}


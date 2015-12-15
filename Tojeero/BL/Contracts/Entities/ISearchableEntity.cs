using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface ISearchableEntity : IUniqueEntity
	{
		IList<string> SearchTokens { get; set; }
	}
}


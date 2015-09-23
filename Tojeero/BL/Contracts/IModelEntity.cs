using System;

namespace Tojeero.Core
{
	public interface IModelEntity : IUniqueEntity
	{
		int SortOrder { get; set; }
	}
}


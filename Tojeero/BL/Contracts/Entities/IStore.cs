using System;

namespace Tojeero.Core
{
	public interface IStore : IModelEntity
	{
		string Name { get; set; }
		string ImageUrl { get; }
		string CategoryID { get; }
	}
}


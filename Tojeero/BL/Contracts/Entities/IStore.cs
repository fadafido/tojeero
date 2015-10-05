using System;

namespace Tojeero.Core
{
	public interface IStore : IModelEntity
	{
		string Name { get; set; }
		Uri ImageUri { get; }
	}
}


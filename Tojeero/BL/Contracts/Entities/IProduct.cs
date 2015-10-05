using System;

namespace Tojeero.Core
{
	public interface IProduct : IModelEntity
	{
		string Name { get; set; }
		double Price { get; set; }
		Uri ImageUri { get; }
		string FormattedPrice { get; }
	}
}


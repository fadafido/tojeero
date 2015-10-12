using System;

namespace Tojeero.Core
{
	public interface IProduct : IModelEntity
	{
		string Name { get; set; }
		double Price { get; set; }
		string ImageUrl { get; }
		string FormattedPrice { get; }
		string CategoryID { get; }
		string SubcategoryID { get; }
	}
}


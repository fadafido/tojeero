using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProduct : IModelEntity, ISearchableEntity
	{
		string Name { get; set; }
		string LowercaseName { get; }
		double Price { get; set; }
		string ImageUrl { get; }
		string FormattedPrice { get; }
		string CategoryID { get; }
		string SubcategoryID { get; }
		int? CityId { get; }
		int? CountryId { get; }
	}
}


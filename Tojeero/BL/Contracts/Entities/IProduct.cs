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

		string StoreID { get; }

		int? CityId { get; }

		int? CountryId { get; }

		IList<string> Tags { get; set; }

		string TagList{ get; }

		IProductCategory Category { get; }

		IProductSubcategory Subcategory { get; }

		IStore Store { get; }

		bool? IsFavorite { get; set; }
	}
}


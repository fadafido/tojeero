using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProduct : IModelEntity, ISearchableEntity, ISocialObject
	{
		string Name { get; set; }

		string LowercaseName { get; }

		double Price { get; set; }

		string ImageUrl { get; }

		string FormattedPrice { get; }

		string CategoryID { get; }

		string SubcategoryID { get; }

		string StoreID { get; }

		string Description { get; set; }

		string CityId { get; }

		ICity City { get; }

		string CountryId { get; }

		ICountry Country { get; }

		IList<string> Tags { get; set; }

		string TagList{ get; }

		IProductCategory Category { get; }

		IProductSubcategory Subcategory { get; }

		IStore Store { get; }
	}
}


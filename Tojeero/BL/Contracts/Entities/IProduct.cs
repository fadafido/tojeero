using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IProduct : IModelEntity, ISearchableEntity, ISocialObject, IMultiImageEntity
	{
		string Name { get; set; }

		string LowercaseName { get; set; }

		double Price { get; set; }

		string ImageUrl { get; }

		string FormattedPrice { get; }

		string CategoryID { get; set; }

		string SubcategoryID { get; set; }

		string StoreID { get; set; }

		string Description { get; set; }

		ProductStatus Status { get; set; }

		bool NotVisible { get; set; }

		string DisapprovalReason { get; }

		string CityId { get; set; }

		ICity City { get; }

		string CountryId { get; set; }

		ICountry Country { get; }

		IList<string> Tags { get; set; }

		string TagList{ get; }

		IProductCategory Category { get; }

		IProductSubcategory Subcategory { get; }

		IStore Store { get; }

		Task Save();

		Task SetMainImage(IImage image);
	}
}


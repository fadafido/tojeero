using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductFilter
	{
		IProductCategory Category { get; set; }
		IProductSubcategory Subcategory { get; set; }
		ICountry Country { get; set; }
		ICity City { get; set; }
		double? StartPrice { get; set; }
		double? EndPrice { get; set; }
		IList<ITag> Tags { get; set; }
	}
}


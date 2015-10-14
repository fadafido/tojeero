using System;
using System.Collections.Generic;

namespace Tojeero.Core
{

	public class ProductFilter : IProductFilter
	{
		public IProductCategory Category { get; set; }
		public IProductSubcategory Subcategory { get; set; }
		public ICountry Country { get; set; }
		public ICity City { get; set; }
		public double? StartPrice { get; set; }
		public double? EndPrice { get; set; }
		public IList<ITag> Tags { get; set; }
	}
}

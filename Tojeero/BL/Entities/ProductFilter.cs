using System;
using System.Collections.Generic;

namespace Tojeero.Core
{

	public class ProductFilter : IProductFilter
	{
		public string CategoryID { get; set; }
		public string SubcategoryID { get; set; }
		public int? CountryId { get; set; }
		public int? CityId { get; set; }
		public double? StartPrice { get; set; }
		public double? EndPrice { get; set; }
		public IList<string> Tags { get; set; }
	}
}

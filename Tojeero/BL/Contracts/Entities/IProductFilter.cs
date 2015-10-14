using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IProductFilter
	{
		string CategoryID { get; set; }
		string SubcategoryID { get; set; }
		int? CountryId { get; set; }
		int? CityId { get; set; }
		double? StartPrice { get; set; }
		double? EndPrice { get; set; }
		IList<string> Tags { get; set; }
	}
}


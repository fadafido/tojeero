using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Tojeero.Core
{
	public interface IProductFilter : INotifyPropertyChanged
	{
		IProductCategory Category { get; set; }
		IProductSubcategory Subcategory { get; set; }
		ICountry Country { get; set; }
		ICity City { get; set; }
		double? StartPrice { get; set; }
		double? EndPrice { get; set; }
		ObservableCollection<string> Tags { get; set; }
		IProductFilter Clone();
	    void SetCountryID(string countryId);
	    void SetCityID(string cityId);

	}
}


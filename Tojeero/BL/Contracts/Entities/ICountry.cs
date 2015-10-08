using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface ICountry : IUniqueEntity
	{
		string Name { get; }
		string Currency { get; }
		string CountryPhoneCode { get; }
		int CountryId { get; }
		ICity[] Cities { get; }
		Task LoadCities();
	}
}


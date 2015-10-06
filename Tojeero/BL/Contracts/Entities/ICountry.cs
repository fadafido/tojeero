using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface ICountry : IUniqueEntity
	{
		string Name { get; }
		string Currency { get; }
		string CountryPhoneCode { get; }
	}
}


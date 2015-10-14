using System;

namespace Tojeero.Core
{
	public interface ICity
	{
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
		int CityId { get; set; }
	}
}


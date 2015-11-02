using System;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public interface IStore : IModelEntity, ISearchableEntity
	{
		string Name { get; set; }
		string Description { get; set; }
		string LowercaseName { get; }
		string ImageUrl { get; }
		string CategoryID { get; }
		int? CityId { get; }
		int? CountryId { get; }
		IStoreCategory Category { get; }

		bool? IsFavorite { get; set; }
	}
}


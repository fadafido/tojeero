using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IStore : IModelEntity, ISearchableEntity, ISocialObject
	{
		string Name { get; set; }

		string Description { get; set; }

		string LowercaseName { get; }

		string ImageUrl { get; }

		string CategoryID { get; }

		IStoreCategory Category { get; }

		string CityId { get; }

		ICity City { get; }

		string CountryId { get; }

		ICountry Country { get; }

		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset);
	}
}


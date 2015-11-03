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
		int? CityId { get; }
		int? CountryId { get; }
		IStoreCategory Category { get; }

		Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset);
	}
}


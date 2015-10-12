using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class CityManager : ICityManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public CityManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region ICityManager implementation

		public Task<IEnumerable<ICity>> Fetch(int countryId)
		{
			return _manager.Fetch<ICity, City>(new FetchCitiesQuery(countryId, _manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}


		public Task ClearCache()
		{
			return _manager.Cache.Clear<City>();
		}

		#endregion
	}

	#region Queries

	public class FetchCitiesQuery : IQueryLoader<ICity>
	{
		IModelEntityManager manager;
		int countryId;

		public FetchCitiesQuery(int countryId, IModelEntityManager manager)
		{
			this.countryId = countryId;
			this.manager = manager;
		}

		public string ID
		{
			get
			{
				return "cities-c" + countryId;
			}
		}

		public async Task<IEnumerable<ICity>> LocalQuery()
		{
			return await manager.Cache.FetchCities(countryId);
		}

		public async Task<IEnumerable<ICity>> RemoteQuery()
		{
			return await manager.Rest.FetchCities(countryId);
		}
	}

	#endregion
}


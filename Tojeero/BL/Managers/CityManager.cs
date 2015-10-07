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

		public Task<IEnumerable<ICity>> FetchCities()
		{
			return _manager.FetchCities();
		}


		public Task ClearCache()
		{
			return _manager.Cache.Clear<City>();
		}

		#endregion
	}
}


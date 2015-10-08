using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class CountryManager : ICountryManager
	{
		#region Private fields and properties

		private readonly IModelEntityManager _manager;

		#endregion

		#region Constructors

		public CountryManager(IModelEntityManager manager)
			: base()
		{
			this._manager = manager;
		}

		#endregion

		#region ICountryManager implementation

		public Task<IEnumerable<ICountry>> FetchCountries()
		{
			return _manager.FetchCountries();
		}


		public Task ClearCache()
		{
			return _manager.Cache.Clear<Country>();
		}

		#endregion
	}
}


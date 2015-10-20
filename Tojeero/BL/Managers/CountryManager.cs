﻿using System;
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

		public Task<IEnumerable<ICountry>> Fetch()
		{
			return _manager.Fetch<ICountry, Country>(new FetchCountriesQuery(_manager), Constants.StoresCacheTimespan.TotalMilliseconds);
		}

		public Task ClearCache()
		{
			return _manager.Cache.Clear<Country>();
		}

		public ICountry Create()
		{
			return new Country();
		}

		#endregion
	}

	#region Queries

	public class FetchCountriesQuery : IQueryLoader<ICountry>
	{
		IModelEntityManager manager;

		public FetchCountriesQuery(IModelEntityManager manager)
		{
			this.manager = manager;
		}

		public string ID
		{
			get
			{
				return "countries";
			}
		}

		public async Task<IEnumerable<ICountry>> LocalQuery()
		{
			return await manager.Cache.FetchCountries();
		}

		public async Task<IEnumerable<ICountry>> RemoteQuery()
		{
			return await manager.Rest.FetchCountries();
		}

		public async Task PostProcess(IEnumerable<ICountry> items)
		{
		}
	}

	#endregion
}

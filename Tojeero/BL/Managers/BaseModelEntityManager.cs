using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Tojeero.Core
{
	public class BaseModelEntityManager : IModelEntityManager
	{
		#region Private Fields and Properties

		private readonly IRestRepository _restRepository;
		private readonly ICacheRepository _cacheRepository;

		#endregion

		#region Constructors

		public BaseModelEntityManager(ICacheRepository cacheRepository, IRestRepository restRepository)
		{
			this._restRepository = restRepository;
			this._cacheRepository = cacheRepository;
		}

		#endregion

		#region IModelEntityManager implementation

		public IRestRepository Rest
		{
			get
			{
				return _restRepository;
			}
		}

		public ICacheRepository Cache
		{
			get
			{
				return _cacheRepository;
			}
		}

		#endregion

		#region IRepository implementation

		public Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			return Rest.FetchProducts(pageSize, offset);
		}

		public Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			return Rest.FetchStores(pageSize, offset);
		}



		#endregion
	}
}


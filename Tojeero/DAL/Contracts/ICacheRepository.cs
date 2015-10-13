using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Threading;

namespace Tojeero.Core
{
	public interface ICacheRepository : IRepository
	{
		#region Generic Methods

		Task Initialize();

		Task<T> FetchAsync<T>(string ID) where T : IUniqueEntity, new();

		Task<T> FetchObjectAsync<T>(object primaryKey) where T : new();

		Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset, string orderBy = null) where T : new();

		Task SaveAsync<T>(T entity);

		Task SaveAsync<T>(IEnumerable<T> collection);

		Task DeleteAsync<T>(string ID) where T : IModelEntity, new();

		Task DeleteAsync<T>(object primaryKey);

		Task DeleteAsync<T>(IEnumerable<string> ID) where T : IModelEntity, new();

		Task DeleteAsync<T>(T entity);

		Task DeleteAsync<T>(IEnumerable<T> entities);

		Task Clear();

		Task Clear<T>();

		Task SaveSearchTokens(IEnumerable<ISearchableEntity> items, string entityType);

		#endregion

		#region Cached Query

		Task<bool> IsEntityCachedExpired(string cacheName);

		#endregion

	}
}


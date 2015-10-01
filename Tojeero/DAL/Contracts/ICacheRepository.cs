using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using System.Threading;

namespace Tojeero.Core
{
	public interface ICacheRepository : IRepository
	{
		Task Initialize();

		Task<T> FetchAsync<T>(int pageSize, int offset);
		Task<T> FetchAsync<T>(int pageSize, int offset, CancellationToken token);


//		Task<T> FetchAsync<T>(string ID) where T : IModelEntity, new ();
//		Task<IEnumerable<T>> FetchAsync<T>() where T : new ();
//
//		Task SaveAsync<T>(T entity);
//		Task SaveAsync<T>(IEnumerable<T> collection);
//
//		Task DeleteAsync<T>(string ID) where T : IModelEntity, new ();
//		Task DeleteAsync<T>(IEnumerable<string> ID) where T : IModelEntity, new ();
//
//		Task DeleteAsync<T>(T entity);
//		Task DeleteAsync<T>(IEnumerable<T> entities);
//
//		Task Clear();
//		Task Clear<T>();
	}
}


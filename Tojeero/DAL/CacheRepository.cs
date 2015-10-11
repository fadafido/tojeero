using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Threading;
using Cirrious.MvvmCross.Community.Plugins.Sqlite;
using Tojeero.Core.Services;
using Tojeero.Core.Toolbox;
using Nito.AsyncEx;

namespace Tojeero.Core
{
	public class CacheRepository : ICacheRepository
	{
		#region Private fields

		private const int TIMEOUT_SECONDS = 10;
		private readonly ISQLiteConnectionFactory _factory;
		private readonly IDeviceContextService _deviceContext;

		AsyncReaderWriterLock readerWriterLock = new AsyncReaderWriterLock();
		SemaphoreSlim _locker = new SemaphoreSlim(1, 1);

		#endregion

		#region Constructors

		public CacheRepository(ISQLiteConnectionFactory factory, IDeviceContextService deviceContext)
		{
			_factory = factory;
			_deviceContext = deviceContext;
		}

		#endregion

		#region IRepository implementation

		public async Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset)
		{
			var result = await FetchAsync<Product>(pageSize, offset, "Name").ConfigureAwait(false);
			return result.Cast<IProduct>();
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			var result = await FetchAsync<Store>(pageSize, offset, "Name").ConfigureAwait(false);
			return result.Cast<IStore>();
		}

		public async Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset, string orderBy = null) where T : new()
		{
			return await fetchAsync<T>(pageSize, offset, orderBy).ConfigureAwait(false);	
		}

		public async Task<IEnumerable<ICountry>> FetchCountries()
		{
			var result = await fetchAsync<Country>(-1, -1).ConfigureAwait(false);	
			return result;
		}

		public Task<IEnumerable<ICity>> FetchCities(int countryId)
		{
			return Task<IEnumerable<ICity>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var result = connection.Table<City>().Where(c => c.CountryId == countryId).ToList();
						return result;
					}
				});
		}

		#endregion

		#region ICacheRepository implementation

		#region Generic Methods

		public Task Initialize()
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.CreateTable<Product>();
						connection.CreateTable<Store>();
						connection.CreateTable<CachedQuery>();
						connection.CreateTable<Country>();
						connection.CreateTable<City>();
					}
				});
		}

		public Task<T> FetchAsync<T>(string id) where T : IUniqueEntity, new()
		{
			return Task<T>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var result = connection.Table<T>().Where(x => x.ID == id).FirstOrDefault();
						return result;
					}
				});
		}

		public Task<T> FetchObjectAsync<T>(object primaryKey) where T : new()
		{
			return Task<T>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						T result = default(T);
						//When there is no object with specified primary key exception is thrown so we need to handle that
						try
						{
							result = connection.Get<T>(primaryKey);
						}
						catch (InvalidOperationException ex)
						{
							
						}
						catch (Exception ex)
						{
							throw ex;
						}
						return result;
					}
				});
		}

		public Task SaveAsync<T>(T item)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.InsertOrReplace(item);
					}
				});
		}

		public Task SaveAsync<T>(IEnumerable<T> items)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.RunInTransaction(() =>
							{
								foreach (T item in items)
								{
									connection.InsertOrReplace(item);
								}
							});
					}
				});
		}

		public Task DeleteAsync<T>(string id) where T : IModelEntity, new()
		{
			return Task<int>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						return connection.Delete(new T() { ID = id });
					}
				});
		}

		public Task DeleteAsync<T>(object primaryKey)
		{
			return Task<int>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						return connection.Delete<T>(primaryKey);
					}
				});
		}

		public Task DeleteAsync<T>(IEnumerable<string> items) where T : IModelEntity, new()
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.RunInTransaction(() =>
							{
								foreach (string id in items)
								{
									connection.Delete(new T() { ID = id });
								}
							});
					}
				});
		}

		public Task DeleteAsync<T>(T item)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.Delete(item);
					}
				});
		}

		public Task DeleteAsync<T>(IEnumerable<T> items)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.RunInTransaction(() =>
							{
								foreach (T item in items)
								{
									connection.Delete(item);
								}
							});
					}
				});
		}


		public Task Clear<T>()
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.WriterLock(source.Token))
					{
						clear<T>();
					}
				});
		}

		public Task Clear()
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					{
						var name = CachedQuery.GetEntityCacheName<Product>();
						if (isEntityCacheExpired(name))
							clear<Product>();

						name = CachedQuery.GetEntityCacheName<Store>();
						if (isEntityCacheExpired(name))
							clear<Store>();
					}
				});
			
		}

		#endregion

		#region Cached Query

		public Task<bool> IsEntityCachedExpired(string cacheName)
		{
			return Task<bool>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					{
						return isEntityCacheExpired(cacheName);
					}
				});
		}

		#endregion

		#endregion

		#region Utility Methods

		private ISQLiteConnection getConnection()
		{
			var connection = _factory.Create(PortablePath.Combine(_deviceContext.CacheFolder.Path, Constants.DatabaseFileName));
			return connection;
		}

		private Task<IEnumerable<T>> fetchAsync<T>(int pageSize, int offset, string orderBy = null) where T : new()
		{
			return Task<IEnumerable<T>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var tableName = typeof(T).GetLocalTableName();
						string query = string.Format("SELECT * FROM [{0}]", tableName);
						if (!string.IsNullOrEmpty(orderBy))
						{
							query += " ORDER BY " + orderBy;
						}
						if (pageSize > 0 && offset >= 0)
						{
							query += string.Format(" LIMIT {0} OFFSET {1}", pageSize, offset);
						}
						var result = connection.Query<T>(query);
						return result;
					}
				});
		}

		private void clear<T>()
		{
			using (var connection = getConnection())
			{
				var cacheName = CachedQuery.GetEntityCacheName<T>();
				var cachedQueries = connection.Table<CachedQuery>().Where(q => q.EntityName == cacheName);
				connection.RunInTransaction(() =>
					{
						foreach (CachedQuery item in cachedQueries)
						{
							connection.Delete(item);
						}
						connection.DeleteAll<T>();
					});	
			}
		}

		private async Task deleteDatabase()
		{
			try
			{
				var folder = _deviceContext.CacheFolder;
				var exists = await folder.CheckExistsAsync(Constants.DatabaseFileName).ConfigureAwait(false);
				if (exists == ExistenceCheckResult.FileExists)
				{
					var dbFile = await folder.GetFileAsync(Constants.DatabaseFileName).ConfigureAwait(false);
					await dbFile.DeleteAsync().ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				Tools.Logger.Log(ex, "Error occured while deleting database from local cache.", LoggingLevel.Warning, true);
			}
		}


		private bool isEntityCacheExpired(string cacheName)
		{
			using (var connection = getConnection())
			{
				//Get the first query that has been cached.
				var queryCache = connection.Table<CachedQuery>().Where(q => q.EntityName == cacheName).OrderBy(q => q.LastFetchedAt).FirstOrDefault();
				//If it's expired then we consider that the whole entity cache is expired
				return queryCache != null && queryCache.IsExpired;
			}
		}

		#endregion
	}
}


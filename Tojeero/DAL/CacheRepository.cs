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
			var result = await FetchAsync<Product>(pageSize, offset).ConfigureAwait(false);
			return result.Cast<IProduct>();
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			var result = await FetchAsync<Store>(pageSize, offset).ConfigureAwait(false);
			return result.Cast<IStore>();
		}

		public async Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset) where T : new()
		{
			using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
			{
				return await fetchAsync<T>(pageSize, offset, source.Token).ConfigureAwait(false);	
			}
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


		public Task Clear<T>(string cacheName)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.UpgradeableReaderLock(source.Token))
					{
						using (var connection = getConnection())
						{
							var cachedQueries = connection.Table<CachedQuery>().Where(q => q.EntityName == cacheName);
							using(var writerLock = readerLock.Upgrade())
							{
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
					}
				});
		}

		public async Task Clear()
		{
			using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
			using (var writerLock = await readerWriterLock.WriterLockAsync(source.Token))
			{
				await deleteDatabase().ConfigureAwait(false);
			}
			await Initialize().ConfigureAwait(false);
		}

		#endregion

		#endregion

		#region Utility Methods

		private ISQLiteConnection getConnection()
		{
			var connection = _factory.Create(PortablePath.Combine(_deviceContext.CacheFolder.Path, Constants.DatabaseFileName));
			return connection;
		}

		private Task<IEnumerable<T>> fetchAsync<T>(int pageSize, int offset, CancellationToken token) where T : new()
		{
			return Task<IEnumerable<T>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var tableName = typeof(T).GetLocalTableName();
						var result = connection.Query<T>(string.Format("SELECT * FROM [{0}]  LIMIT {1} OFFSET {2}", tableName, pageSize, offset));
						return result;
					}
				});
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

		#endregion
	}
}


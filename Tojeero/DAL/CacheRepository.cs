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
	public class SampleEntity : BaseModelEntity
	{

	}

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

		public Task Initialize()
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.CreateTable<SampleEntity>();
//						connection.CreateTable<Product>();
//						connection.CreateTable<Store>();
//						connection.CreateTable<CachedQuery>();
					}
				});
		}

		public Task<T> FetchAsync<T>(string id) where T : IModelEntity, new()
		{
			return Task<T>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var result = connection.Table<T>().Where(x => x.ObjectId == id).FirstOrDefault();
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
						return connection.Get<T>(primaryKey);
					}
				});
		}

		public Task SaveAsync(object item)
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

		public Task SaveAsync<T>(T item)
		{
			return SaveAsync(item);
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
						return connection.Delete(new T() { ObjectId = id });
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
									connection.Delete(new T() { ObjectId = id });
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
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						connection.DeleteAll<T>();
					}
				});
		}

		public async Task Clear()
		{
			await Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					{
						deleteDatabase();
					}
				}).ConfigureAwait(false);
			await Initialize().ConfigureAwait(false);
		}

			
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
						var result = connection.Query<T>("SELECT * FROM {0}  LIMIT {1} OFFSET {2}", tableName, pageSize, offset);
						return result;
					}
				});
		}

		private void deleteDatabase()
		{
			try
			{
				var folder = _deviceContext.CacheFolder;
				var exists = folder.CheckExistsAsync(Constants.DatabaseFileName).Result;
				if (exists == ExistenceCheckResult.FileExists)
				{
					var dbFile = folder.GetFileAsync(Constants.DatabaseFileName).Result;
					dbFile.DeleteAsync().Wait();
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


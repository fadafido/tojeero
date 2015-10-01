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

namespace Tojeero.Core
{
	public class CacheRepository : ICacheRepository
	{
		#region Private fields
		private const int TIMEOUT_SECONDS = 10;
		private readonly ISQLiteConnectionFactory _factory;
		private readonly IDeviceContextService _deviceContext;

		ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
		SemaphoreSlim _locker = new SemaphoreSlim(1,1);

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
			var result = await FetchAsync<Product>(pageSize, offset);
			return result.Cast<IProduct>();
		}

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset)
		{
			var result = await FetchAsync<Store>(pageSize, offset);
			return result.Cast<IStore>();
		}

		public async Task<IEnumerable<T>> FetchAsync<T>(int pageSize, int offset) where T : new()
		{
			using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
			{
				return await fetchAsync<T>(pageSize, offset, source.Token);	
			}
		}

		#endregion

		#region ICacheRepository implementation

		public Task Initialize()
		{
			return Task.Factory.StartNew(() =>
				{
					_locker.Wait();
					try
					{
						using(var connection = getConnection())
						{
							connection.CreateTable<Product>();
							connection.CreateTable<Store>();
						}
					}
					finally
					{
						_locker.Release();
					}
				});
		}

		public Task<IEnumerable<T>> FetchAsync<T> () where T : new()
		{
			return Task<IEnumerable<T>>.Factory.StartNew(() =>
			{
				EnterLock(false);
				try
				{
					return getItems<T>();
				}
				finally
				{
					ExitLock(false);
				}
			});
		}

		public Task<T> FetchAsync<T> (string id) where T : IModelEntity, new ()
		{
			return Task<T>.Factory.StartNew(() =>
			{
				EnterLock(false);
				try
				{
					return getItem<T>(id);
				}
				finally
				{
					ExitLock(false);
				}
			});
		}

		public Task SaveAsync<T> (T item)
		{
			return Task.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					saveItem<T>(item);
				}
				finally
				{
					ExitLock(true);
				}
			});
		}

		public Task SaveAsync<T> (IEnumerable<T> items)
		{
			return Task.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					saveItems<T>(items);
				}
				finally
				{
					ExitLock(true);
				}
			});
		}

		public Task DeleteAsync<T>(string id) where T : IModelEntity, new ()
		{
			return Task<int>.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					return deleteItem<T>(id);
				}
				finally
				{
					ExitLock(true);
				}
			});
		}
			
		public Task DeleteAsync<T> (IEnumerable<string> items) where T : IModelEntity, new()
		{
			return Task.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					deleteItems<T>(items);
				}
				finally
				{
					ExitLock(true);
				}
			});
		}

		public Task DeleteAsync<T> (T item)
		{
			return Task.Factory.StartNew(() =>
				{
					
					EnterLock(true);
					try
					{
						deleteItem<T>(item);
					}
					finally
					{
						ExitLock(true);
					}
				});
		}

		public Task DeleteAsync<T> (IEnumerable<T> items)
		{
			return Task.Factory.StartNew(() =>
				{
					EnterLock(true);
					try
					{
						deleteItems<T>(items);
					}
					finally
					{
						ExitLock(true);
					}
				});
		}


		public Task Clear<T>()
        {
			return Task.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					 clearTable<T>();
				}
				finally
				{
					ExitLock(true);
				}
			});
		}

		public Task Clear()
        {
			return Task.Factory.StartNew(() =>
			{
				EnterLock(true);
				try
				{
					deleteDatabase();
					Initialize();
				}
				finally
				{
					ExitLock(true);
				}
			});
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
					_locker.Wait(token);
					if(token.IsCancellationRequested)
						throw new OperationCanceledException("Fetching was cancelled.");
					try
					{
						using (var connection = getConnection())
						{
							var tableName = typeof(T).GetLocalTableName();
							var result = connection.Query<T>("SELECT * FROM {0}  LIMIT {1} OFFSET {2}", tableName, pageSize, offset);
							return result;
						}
					}
					finally
					{
						_locker.Release();
					}
				});
		}

		private IEnumerable<T> getItems<T> () where T : new()
		{
			using (var connection = getConnection())
			{
				var result = connection.Table<T>();
				return result;
			}
		}

		private T getItem<T> (string id) where T : IModelEntity, new ()
		{
			using (var connection = getConnection())
			{
				var result = connection.Table<T>().Where(x => x.ObjectId == id).FirstOrDefault();
				return result;
			}
		}

		private void saveItem<T> (T item)
		{
			using (var connection = getConnection())
			{
				connection.InsertOrReplace(item);
			}
		}

		private void saveItems<T> (IEnumerable<T> items)
		{
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
		}


		private int deleteItem<T>(string id) where T : IModelEntity, new ()
		{
			using (var connection = getConnection())
			{
				return connection.Delete(new T() { ObjectId = id });
			}
		}

		private void deleteItems<T> (IEnumerable<string> items) where T : IModelEntity, new()
		{
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
		}

		private void deleteItem<T> (T item)
		{
			using (var connection = getConnection())
			{
				connection.Delete(item);
			}
		}

		private void deleteItems<T> (IEnumerable<T> items)
		{
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
		}
			
		private void clearTable<T>()
		{
			using (var connection = getConnection())
			{
				connection.DeleteAll<T>();
			}
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
			

			
		private void EnterLock(bool isWrite)
		{
			if (isWrite)
			{
				readerWriterLock.EnterWriteLock();
			}
			else
			{
				readerWriterLock.EnterReadLock();
			}
		}

		private void ExitLock(bool isWrite)
		{
			if (isWrite)
			{
				readerWriterLock.ExitWriteLock();
			}
			else
			{
				readerWriterLock.ExitReadLock();
			}
		}


		#endregion
	}
}


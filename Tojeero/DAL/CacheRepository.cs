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

		private const int TIMEOUT_SECONDS = 5;
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

		//TODO:Filtering implementation is buggy and is not working now. Need to fix this on next phase.
		public Task<IEnumerable<IProduct>> FetchProducts(int pageSize, int offset, IProductFilter filter = null)
		{
			return Task<IEnumerable<IProduct>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var query = connection.Table<Product>();
						query = getFilteredProductQuery(query, filter, connection);
						var result = query.ToList();
						return result;
					}
				});
		}

		//TODO:Filtering is not implemented, so the filter parameter is no applied yet for this query.
		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, IStoreFilter filter = null)
		{
			var result = await FetchAsync<Store>(pageSize, offset, "LowercaseName").ConfigureAwait(false);
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

		public async Task<IEnumerable<IProductCategory>> FetchProductCategories()
		{
			var result = await fetchAsync<ProductCategory>(-1, -1).ConfigureAwait(false);	
			return result;
		}

		public Task<IEnumerable<IProductSubcategory>> FetchProductSubcategories(string categoryID)
		{
			return Task<IEnumerable<IProductSubcategory>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var result = connection.Table<ProductSubcategory>().Where(p => p.CategoryID == categoryID).ToList();
						return result;
					}
				});
		}

		public async Task<IEnumerable<IStoreCategory>> FetchStoreCategories()
		{
			var result = await fetchAsync<StoreCategory>(-1, -1).ConfigureAwait(false);	
			return result;
		}

		//TODO:Filtering is not implemented, so the filter parameter is no applied yet for this query.
		public async Task<IEnumerable<IProduct>> FindProducts(string searchQuery, int pageSize, int offset, IProductFilter filter = null)
		{
			return await find<Product>(searchQuery, (ITableQuery<Product> q) => q.OrderBy(p => p.LowercaseName), pageSize, offset).ConfigureAwait(false);
		}

		//TODO:Filtering is not implemented, so the filter parameter is no applied yet for this query.
		public async Task<IEnumerable<IStore>> FindStores(string searchQuery, int pageSize, int offset, IStoreFilter filter = null)
		{
			return await find<Store>(searchQuery, (ITableQuery<Store> q) => q.OrderBy(s => s.LowercaseName), pageSize, offset).ConfigureAwait(false);
		}

		public async Task SaveSearchTokens(IEnumerable<ISearchableEntity> items, string entityType)
		{
			if (items == null)
				return;
			foreach (var item in items)
			{
				if (item.SearchTokens != null)
				{
					var tokens = item.SearchTokens.Select(t =>
						{
							var token = new SearchToken()
							{ 						
								EntityID = item.ID,
								EntityType = entityType,
								Token = t
							};
							token.ID = token.EntityID + token.EntityType + token.Token;
							return token;
						});
					await SaveAsync<SearchToken>(tokens);
				}
			}
		}


		public async Task<IEnumerable<ITag>> FetchTags(int pageSize, int offset)
		{
			var result = await FetchAsync<Tag>(pageSize, offset, "Text").ConfigureAwait(false);
			return result.Cast<ITag>();
		}

		public Task<IEnumerable<ITag>> FindTags(string searchQuery, int pageSize, int offset)
		{
			return Task<IEnumerable<ITag>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var result = connection.Table<Tag>().Where(t => t.Text.StartsWith(searchQuery.Trim())).ToList();
						return result;
					}
				});
		}

		public Task SaveProductTags(string productId, IList<string> tags)
		{
			return Task.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var writerLock = readerWriterLock.WriterLock(source.Token))
					using (var connection = getConnection())
					{
						foreach (var t in tags)
						{
							var tag = new ProductTag()
							{
								ID = productId + t,
								ProductID = productId,
								Tag = t
							};
							connection.InsertOrReplace(tag);
						}
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
						connection.CreateTable<SearchToken>();
						connection.CreateTable<Tag>();
						connection.CreateTable<ProductTag>();
						connection.CreateTable<ProductCategory>();
						connection.CreateTable<ProductSubcategory>();
						connection.CreateTable<StoreCategory>();
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

		public Task<IEnumerable<T>> find<T>(string searchQuery, Action<ITableQuery<T>> orderByAction, int pageSize, int offset) where T : ISearchableEntity, new()
		{
			return Task<IEnumerable<T>>.Factory.StartNew(() =>
				{
					using (var source = new CancellationTokenSource(TimeSpan.FromSeconds(TIMEOUT_SECONDS)))
					using (var readerLock = readerWriterLock.ReaderLock(source.Token))
					using (var connection = getConnection())
					{
						var entityName = CachedQuery.GetEntityCacheName<T>();
						var tokens = searchQuery.Tokenize();

						var ids = (from t in connection.Table<SearchToken>()
						           where t.EntityType == entityName && tokens.Contains(t.Token)
						           group t by t.EntityID into g
						           where g.Count() == tokens.Count
						           select g.Key).ToList();
						var query = from p in connection.Table<T>()
						            where ids.Contains(p.ID)
						            select p;
						if (orderByAction != null)
							orderByAction(query);
						if (pageSize > 0 && offset >= 0)
							query = query.Take(pageSize).Skip(offset);

						return query.ToList();

					}
				});
		}

		private void clear<T>()
		{
			using (var connection = getConnection())
			{
				var cacheName = CachedQuery.GetEntityCacheName<T>();
				var cachedQueries = connection.Table<CachedQuery>().Where(q => q.EntityName == cacheName);
				var searchTokens = connection.Table<SearchToken>().Where(q => q.EntityType == cacheName);
				connection.RunInTransaction(() =>
					{
						foreach (var item in cachedQueries)
						{
							connection.Delete(item);
						}
							
						foreach (var t in searchTokens)
						{
							connection.Delete(t);
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

		private ITableQuery<Product> getFilteredProductQuery(ITableQuery<Product> query, IProductFilter filter, ISQLiteConnection connection)
		{
			if (filter != null)
			{
				if (filter.Category != null)
				{
					query = query.Where(p => p.CategoryID == filter.Category.ID);
				}

				if (filter.Subcategory != null)
				{
					query = query.Where(p => p.SubcategoryID == filter.Subcategory.ID);
				}

				if (filter.Country != null)
				{
					query = query.Where(p => p.CountryId == filter.Country.CountryId);
				}

				if (filter.City != null)
				{
					query = query.Where(p => p.CityId == filter.City.CityId);
				}

				if (filter.StartPrice != null)
				{
					query = query.Where(p => p.Price >= filter.StartPrice.Value);
				}

				if (filter.EndPrice != null)
				{
					query = query.Where(p => p.Price <= filter.EndPrice.Value);
				}

				if (filter.Tags != null && filter.Tags.Count > 0)
				{
					var temp = connection.Table<ProductTag>().ToList();
					var temp1 = (from t in connection.Table<ProductTag>()
					            where filter.Tags.Contains(t.Tag)
					            select t).ToList();
					var temp2 = (from t in connection.Table<ProductTag>()
						where filter.Tags.Contains(t.Tag)
						group t by t.ProductID into g
						select g).ToList();
					var productIDs = (from t in connection.Table<ProductTag>()
					                  where filter.Tags.Contains(t.Tag)
					                  group t by t.ProductID into g
					                  where g.Count() == filter.Tags.Count
					                  select g.Key).ToList();
					query = query.Where(p => productIDs.Contains(p.ID));
				}
			}
			return query;
		}

		#endregion
	}
}


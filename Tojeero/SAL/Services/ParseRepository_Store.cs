﻿using System;
using System.Linq;
using System.Reflection;
using Parse;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Tojeero.Core.Toolbox;
using System.Collections;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core
{
	public partial class ParseRepository
	{
		#region IRepository implementation

		public async Task<IEnumerable<IStore>> FetchStores(int pageSize, int offset, IStoreFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var query = new ParseQuery<ParseStore>().Where(s => s.NotVisible == false).OrderBy(s => s.LowercaseName).Include("category").Include("country").Include("city");
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				query = getFilteredStoreQuery(query, filter);
				var result = await query.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Store(s) as IStore);
			}
		}

		public async Task<IEnumerable<IStore>> FetchFavoriteStores(int pageSize, int offset)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchStoresTimeout))
			{
				var user = ParseUser.CurrentUser as TojeeroUser;
				if (user == null)
					return null;
				var query = user.FavoriteStores.Query.Where(s => s.NotVisible == false).OrderBy(p => p.LowercaseName).Include("category").Include("country").Include("city");
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync();	
				return result.Select(p => new Store(p) as IStore);
			}
		}
			
		public async Task<IEnumerable<IStore>> FindStores(string query, int pageSize, int offset, IStoreFilter filter = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FindStoresTimeout))
			{
				var parseQuery = new ParseQuery<ParseStore>().Where(s => s.NotVisible == false).OrderBy(s => s.LowercaseName).Include("category").Include("country").Include("city");
				var tokens = query.Tokenize();
				if (tokens != null && tokens.Count > 0)
				{
					parseQuery = getContainsAllQuery(parseQuery, "searchTokens", tokens);
				}
				if (pageSize > 0 && offset >= 0)
				{
					parseQuery = parseQuery.Limit(pageSize).Skip(offset);
				}
				parseQuery = getFilteredStoreQuery(parseQuery, filter);
				var result = await parseQuery.FindAsync(tokenSource.Token).ConfigureAwait(false);
				return result.Select(s => new Store(s) as IStore);
			}
		}

		public async Task<IEnumerable<IProduct>> FetchStoreProducts(string storeID, int pageSize, int offset, bool includeInvisible = false)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.FetchProductsTimeout))
			{
				var store = ParseObject.CreateWithoutData<ParseStore>(storeID);
				var query = new ParseQuery<ParseProduct>().Where(p => p.Store == store).OrderBy(p => p.LowercaseName).Include("category").Include("subcategory").Include("store");
				if (!includeInvisible)
				{
					query = query.Where(p => p.NotVisible == false && p.Status == (int)ProductStatus.Approved);
				}
				if (pageSize > 0 && offset >= 0)
				{
					query = query.Limit(pageSize).Skip(offset);
				}
				var result = await query.FindAsync();	
				return result.Select(p => new Product(p) as IProduct);
			}
		}
			
		public async Task<IStore> SaveStore(ISaveStoreViewModel store)
		{
			if (store == null)
				throw new NullReferenceException("When saving store the ISaveStoreViewModel should be non null");
			using (var tokenSource = new CancellationTokenSource(Constants.SaveStoreTimeout))
			{
				var s = store.CurrentStore != null ? store.CurrentStore : new Store();
				s.CategoryID = store.Category != null ? store.Category.ID : null;
				s.CountryId = store.Country != null ? store.Country.ID : null;
				s.CityId = store.City != null ? store.City.ID : null;
				s.Name = store.Name;
				s.Description = store.Description;
				s.DeliveryNotes = store.DeliveryNotes;
				s.OwnerID = ParseUser.CurrentUser.ObjectId;
				s.LowercaseName = s.Name.ToLower();
				s.SearchTokens = new string[] { s.Name, s.Description }.Tokenize();
				if (store.MainImage.NewImage != null)
				{
					await s.SetMainImage(store.MainImage.NewImage);
				}
				await s.Save();
				return s;
			}
		}

		public async Task<IStore> FetchDefaultStoreForUser(string userID)
		{
			if (string.IsNullOrEmpty(userID))
				return null;
			var user = ParseObject.CreateWithoutData<TojeeroUser>(userID);
			var query = new ParseQuery<ParseStore>().Where(s => s.Owner == user).Include("category").Include("country").Include("city");
			var store = await query.FirstOrDefaultAsync();

			return store != null ? new Store(store) : null;
		}

		public async Task<bool> CheckStoreNameIsReserved(string storeName, string currentStoreID = null)
		{
			using (var tokenSource = new CancellationTokenSource(Constants.DefaultTimeout))
			{
				var query = new ParseQuery<ReservedName>().Where(r => r.Name == storeName && r.Type == (int)ReservedNameType.Store);
				var result = await query.FirstOrDefaultAsync(tokenSource.Token).ConfigureAwait(false);
				if (result != null)
					return true;
				var storeQuery = new ParseQuery<ParseStore>().Where(s => s.LowercaseName == storeName).Select("objectId");
				if (!string.IsNullOrEmpty(currentStoreID))
				{
					storeQuery = storeQuery.Where(s => s.ObjectId != currentStoreID);
				}
				var store = await storeQuery.FirstOrDefaultAsync(tokenSource.Token).ConfigureAwait(false);
				if (store != null)
					return true;
				return false;
			}
		}

		#endregion

		#region Utility methods

		private ParseQuery<ParseStore> getFilteredStoreQuery(ParseQuery<ParseStore> query, IStoreFilter filter)
		{
			if (filter != null)
			{
				if (filter.Category != null)
				{
					query = query.Where(s => s.Category == ParseObject.CreateWithoutData<ParseStoreCategory>(filter.Category.ID));
				}

				if (filter.Country != null)
				{
					query = query.Where(s => s.Country == ParseObject.CreateWithoutData<ParseCountry>(filter.Country.ID));
				}

				if (filter.City != null)
				{
					query = query.Where(s => s.City == ParseObject.CreateWithoutData<ParseCity>(filter.City.ID));
				}

				if (filter.Tags != null && filter.Tags.Count > 0)
				{
					query = getContainsAllQuery(query, "tags", filter.Tags);
				}
			}
			return query;
		}
			
		#endregion
	}
}


﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tojeero.Core.ViewModels;

namespace Tojeero.Core
{
	public interface IProductManager : IBaseModelEntityManager
	{
		Task<IEnumerable<IProduct>> Fetch(int pageSize, int offset, IProductFilter filter = null);
		Task<IEnumerable<IProduct>> FetchFavorite(int pageSize, int offset);
		Task<int> CountFavorite();
		Task<IEnumerable<IProduct>> Find(string query, int pageSize, int offset, IProductFilter filter = null);
		Task<int> Count(string query, IProductFilter filter = null);
		Task<IProduct> Save(ISaveProductViewModel store);
	}
}


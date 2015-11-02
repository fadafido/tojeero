using System;
using Tojeero.Core.ViewModels;
using Tojeero.Core;
using Tojeero.Core.Toolbox;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Forms
{
	public class FavoriteStoresViewModel : BaseCollectionViewModel<StoreViewModel>
	{
		#region Private fields and properties

		private readonly IStoreManager _manager;

		private static Comparison<StoreViewModel> _comparer;
		public static Comparison<StoreViewModel> Comparer
		{
			get
			{
				if (_comparer == null)
				{
					_comparer = new Comparison<StoreViewModel>((x, y) =>
						{
							if(x.Store == null || y.Store == null)
								return -1;
							if(x.Store.ID == y.Store.ID)
								return 0;
							if(x.Store.LowercaseName == null || y.Store.LowercaseName == null)
								return -1;
							return x.Store.LowercaseName.CompareIgnoreCase(y.Store.LowercaseName);
						});
				}
				return _comparer;
			}
		}

		#endregion

		#region Constructors

		public FavoriteStoresViewModel(IStoreManager manager)
			: base(new FavoriteStoresQuery(manager), Constants.StoresPageSize)
		{
			_manager = manager;
		}

		#endregion

		#region Queries

		private class FavoriteStoresQuery : IModelQuery<StoreViewModel>
		{
			IStoreManager manager;
			public FavoriteStoresQuery (IStoreManager manager)
			{
				this.manager = manager;

			}

			public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.FetchFavorite(pageSize, offset);
				return result.Select(p => new StoreViewModel(p));
			}

			public Comparison<StoreViewModel> Comparer
			{
				get
				{
					return FavoriteStoresViewModel.Comparer;
				}
			}

			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		#endregion
	}
}


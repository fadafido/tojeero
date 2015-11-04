using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;

namespace Tojeero.Core.ViewModels
{
	public class FavoriteProductsViewModel : BaseCollectionViewModel<ProductViewModel>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;

		#endregion

		#region Constructors

		public FavoriteProductsViewModel(IProductManager manager)
			: base(new FavoriteProductsQuery(manager), Constants.ProductsPageSize)
		{
			_manager = manager;
		}

		#endregion

		#region Queries

		private class FavoriteProductsQuery : IModelQuery<ProductViewModel>
		{
			IProductManager manager;
			public FavoriteProductsQuery (IProductManager manager)
			{
				this.manager = manager;

			}

			public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.FetchFavorite(pageSize, offset);
				return result.Select(p => new ProductViewModel(p));
			}

			public Comparison<ProductViewModel> Comparer
			{
				get
				{
					return Comparers.ProductName;
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


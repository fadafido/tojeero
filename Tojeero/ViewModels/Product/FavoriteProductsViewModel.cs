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

		private static Comparison<ProductViewModel> _comparer;
		public static Comparison<ProductViewModel> Comparer
		{
			get
			{
				if (_comparer == null)
				{
					_comparer = new Comparison<ProductViewModel>((x, y) =>
						{
							if(x.Product == null || y.Product == null)
								return -1;
							if(x.Product.ID == y.Product.ID)
								return 0;
							if(x.Product.LowercaseName == null || y.Product.LowercaseName == null)
								return -1;
							return x.Product.LowercaseName.CompareIgnoreCase(y.Product.LowercaseName);
						});
				}
				return _comparer;
			}
		}
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
					return FavoriteProductsViewModel.Comparer;
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


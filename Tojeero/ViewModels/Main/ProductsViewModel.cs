using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseSearchViewModel<IProduct>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;

		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager)
			: base()
		{
			_manager = manager;
		}

		#endregion

		#region implemented abstract members of BaseSearchViewModel

		protected override BaseCollectionViewModel<IProduct> GetBrowsingViewModel()
		{
			return new BaseCollectionViewModel<IProduct>(new ProductsQuery(_manager), Constants.ProductsPageSize);
		}

		protected override BaseCollectionViewModel<IProduct> GetSearchViewModel(string searchQuery)
		{
			return new BaseCollectionViewModel<IProduct>(new SearchProductsQuery(searchQuery, _manager), Constants.ProductsPageSize);
		}

		#endregion

		#region Queries

		private class ProductsQuery : IModelQuery<IProduct>
		{
			IProductManager manager;
			public ProductsQuery (IProductManager manager)
			{
				this.manager = manager;
				
			}

			public Task<IEnumerable<IProduct>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Fetch(pageSize, offset);
			}

			private Comparison<IProduct> _comparer;
			public Comparison<IProduct> Comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparison<IProduct>((x, y) =>
							{
								if(x.ID == y.ID)
									return 0;
								return string.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase);
							});
					}
					return _comparer;
				}
			}
			
			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchProductsQuery : IModelQuery<IProduct>
		{
			IProductManager manager;
			string searchQuery;

			public SearchProductsQuery (string searchQuery, IProductManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public Task<IEnumerable<IProduct>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Find(searchQuery, pageSize, offset);
			}

			private Comparison<IProduct> _comparer;
			public Comparison<IProduct> Comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparison<IProduct>((x, y) =>
							{
								if(x.ID == y.ID)
									return 0;
								return x.Name.CompareTo(y.Name);
							});
					}
					return _comparer;
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


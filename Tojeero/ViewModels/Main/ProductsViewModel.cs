using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseCollectionViewModel<IProduct>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;

		#endregion

		#region Properties

		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager)
			: base(new ProductsQuery(manager), manager.ClearCache, Constants.ProductsPageSize)
		{
			_manager = manager;
		}

		#endregion

		#region Utility

		private class ProductsQuery : IModelQuery<IProduct>
		{
			IProductManager manager;
			public ProductsQuery (IProductManager manager)
			{
				this.manager = manager;
				
			}

			public System.Threading.Tasks.Task<IEnumerable<IProduct>> Fetch(int pageSize = -1, int offset = -1)
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
								return x.Name.CompareTo(y.Name);
							});
					}
					return _comparer;
				}
			}
			
		}

		#endregion
	}
}


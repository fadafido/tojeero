using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tojeero.Core.ViewModels
{
	public class StoresViewModel : BaseCollectionViewModel<IStore>
	{
		#region Private fields and properties

		private readonly IStoreManager _manager;

		#endregion

		#region Constructors

		public StoresViewModel(IStoreManager manager)
			: base(new StoresQuery(manager), Constants.StoresPageSize)
		{
			_manager = manager;
		}

		#endregion

		#region Utility

		private class StoresQuery : IModelQuery<IStore>
		{
			IStoreManager manager;
			public StoresQuery (IStoreManager manager)
			{
				this.manager = manager;

			}

			public System.Threading.Tasks.Task<IEnumerable<IStore>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Fetch(pageSize, offset);
			}

			private Comparison<IStore> _comparer;
			public Comparison<IStore> Comparer
			{
				get
				{
					if (_comparer == null)
					{
						_comparer = new Comparison<IStore>((x, y) =>
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


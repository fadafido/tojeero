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
	public class StoresViewModel : BaseSearchViewModel<IStore>
	{
		#region Private fields and properties

		private readonly IStoreManager _manager;
		private readonly MvxSubscriptionToken _token;

		#endregion

		#region Constructors

		public StoresViewModel(IStoreManager manager, IMvxMessenger messenger)
			: base()
		{
			_manager = manager;
			_token = messenger.Subscribe<StoreFilterChangedMessage>((m) =>
				{
					this.ViewModel.RefetchCommand.Execute(null);
				});
		}

		#endregion

		#region implemented abstract members of BaseSearchViewModel

		protected override BaseCollectionViewModel<IStore> GetBrowsingViewModel()
		{
			return new BaseCollectionViewModel<IStore>(new StoresQuery(_manager), Constants.StoresPageSize);
		}

		protected override BaseCollectionViewModel<IStore> GetSearchViewModel(string searchQuery)
		{
			return new BaseCollectionViewModel<IStore>(new SearchStoresQuery(searchQuery, _manager), Constants.StoresPageSize);
		}

		#endregion

		#region Queries

		private class StoresQuery : IModelQuery<IStore>
		{
			IStoreManager manager;

			public StoresQuery(IStoreManager manager)
			{
				this.manager = manager;

			}

			public Task<IEnumerable<IStore>> Fetch(int pageSize = -1, int offset = -1)
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
								if (x.ID == y.ID)
									return 0;
								return x.LowercaseName.CompareIgnoreCase(y.LowercaseName);
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

		private class SearchStoresQuery : IModelQuery<IStore>
		{
			IStoreManager manager;
			string searchQuery;

			public SearchStoresQuery(string searchQuery, IStoreManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public Task<IEnumerable<IStore>> Fetch(int pageSize = -1, int offset = -1)
			{
				return manager.Find(searchQuery, pageSize, offset);
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
								if (x.ID == y.ID)
									return 0;
								return x.LowercaseName.CompareIgnoreCase(y.LowercaseName);
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


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
	public class StoresViewModel : BaseSearchViewModel<StoreViewModel>
	{
		#region Private fields and properties

		private readonly IStoreManager _manager;
		private readonly MvxSubscriptionToken _token;

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

		protected override BaseCollectionViewModel<StoreViewModel> GetBrowsingViewModel()
		{
			return new BaseCollectionViewModel<StoreViewModel>(new StoresQuery(_manager), Constants.StoresPageSize);
		}

		protected override BaseCollectionViewModel<StoreViewModel> GetSearchViewModel(string searchQuery)
		{
			return new BaseCollectionViewModel<StoreViewModel>(new SearchStoresQuery(searchQuery, _manager), Constants.StoresPageSize);
		}

		#endregion

		#region Queries

		private class StoresQuery : IModelQuery<StoreViewModel>
		{
			IStoreManager manager;
			public StoresQuery (IStoreManager manager)
			{
				this.manager = manager;

			}

			public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.Fetch(pageSize, offset, RuntimeSettings.StoreFilter);
				return result.Select(p => new StoreViewModel(p));
			}

			public Comparison<StoreViewModel> Comparer
			{
				get
				{
					return StoresViewModel.Comparer;
				}
			}


			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchStoresQuery : IModelQuery<StoreViewModel>
		{
			IStoreManager manager;
			string searchQuery;

			public SearchStoresQuery (string searchQuery, IStoreManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public async Task<IEnumerable<StoreViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.Find(searchQuery, pageSize, offset, RuntimeSettings.StoreFilter);
				return result.Select(p => new StoreViewModel(p));
			}

			public Comparison<StoreViewModel> Comparer
			{
				get
				{
					return StoresViewModel.Comparer;
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


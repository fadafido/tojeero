using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core.Messages;
using Tojeero.Forms.Resources;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseSearchViewModel<ProductViewModel>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;
		private readonly MvxSubscriptionToken _filterChangedToken;
		private readonly MvxSubscriptionToken _sessionChangedToken;
		private readonly MvxSubscriptionToken _productChangeToken;

		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager, IMvxMessenger messenger)
			: base()
		{
			_manager = manager;
			_filterChangedToken = messenger.Subscribe<ProductFilterChangedMessage>((m) =>
				{
					this.RefetchCommand.Execute(null);
				});
			_productChangeToken = messenger.Subscribe<ProductChangedMessage>((message) =>
				{
					this.RefetchCommand.Execute(null);
				});
			_sessionChangedToken = messenger.Subscribe<SessionStateChangedMessage>((m) =>
				{
					this.RefetchCommand.Execute(null);
				});
		}

		#endregion


		#region Parent override

		protected override BaseCollectionViewModel<ProductViewModel> GetBrowsingViewModel()
		{
			var viewModel = new BaseCollectionViewModel<ProductViewModel>(new ProductsQuery(_manager), Constants.ProductsPageSize);
			viewModel.Placeholder = AppResources.MessageNoProducts;
			return viewModel;
		}

		protected override BaseCollectionViewModel<ProductViewModel> GetSearchViewModel(string searchQuery)
		{
			var viewModel = new BaseCollectionViewModel<ProductViewModel>(new SearchProductsQuery(searchQuery, _manager), Constants.ProductsPageSize);
			viewModel.Placeholder = AppResources.MessageNoProducts;
			return viewModel;
		}

		#endregion

		#region Queries

		private class ProductsQuery : IModelQuery<ProductViewModel>
		{
			IProductManager manager;
			public ProductsQuery (IProductManager manager)
			{
				this.manager = manager;
				
			}

			public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.Fetch(pageSize, offset, RuntimeSettings.ProductFilter);
				return result.Select(p => new ProductViewModel(p));
			}

			public Comparison<ProductViewModel> Comparer
			{
				get
				{
					return  Comparers.ProductName;
				}
			}

			
			public Task ClearCache()
			{
				return manager.ClearCache();
			}
		}

		private class SearchProductsQuery : IModelQuery<ProductViewModel>
		{
			IProductManager manager;
			string searchQuery;

			public SearchProductsQuery (string searchQuery, IProductManager manager)
			{
				this.searchQuery = searchQuery;
				this.manager = manager;
			}

			public async Task<IEnumerable<ProductViewModel>> Fetch(int pageSize = -1, int offset = -1)
			{
				var result = await manager.Find(searchQuery, pageSize, offset, RuntimeSettings.ProductFilter);
				return result.Select(p => new ProductViewModel(p));
			}
				
			public Comparison<ProductViewModel> Comparer
			{
				get
				{
					return null;
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


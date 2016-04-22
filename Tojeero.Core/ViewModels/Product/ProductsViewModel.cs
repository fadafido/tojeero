using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;
using Tojeero.Core;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Messages;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.Product
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

		#region Properties

		public Action ShowFiltersAction { get; set; }
		public Action<ListMode> ChangeListModeAction { get; set; }

		private ListMode _listMode = Settings.ProductListMode;

		public ListMode ListMode
		{ 
			get
			{
				return _listMode; 
			}
			private set
			{
				if (_listMode != value)
				{
					_listMode = value; 
					updateListMode();
					RaisePropertyChanged(() => ListMode); 
					RaisePropertyChanged(() => ListModeIcon);
				}

			}
		}

		public string ListModeIcon
		{
			get
			{
				return this.ListMode == ListMode.Normal ? "listLargeCellIcon.png" : "listCellIcon.png";
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _filterCommand;

		public System.Windows.Input.ICommand FilterCommand
		{
			get
			{
				_filterCommand = _filterCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowFiltersAction.Fire();
				});
				return _filterCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _toggleListModeCommand;

		public System.Windows.Input.ICommand ToggleListModeCommand
		{
			get
			{
				_toggleListModeCommand = _toggleListModeCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(()=>{
					this.ListMode = this.ListMode == ListMode.Normal ? ListMode.Large : ListMode.Normal;
				});
				return _toggleListModeCommand;
			}
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
					return  null;
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

		#region Utility methods

		private void updateListMode()
		{
			Settings.ProductListMode = this.ListMode;
			ChangeListModeAction.Fire(this.ListMode);
		}

		#endregion
	}
}


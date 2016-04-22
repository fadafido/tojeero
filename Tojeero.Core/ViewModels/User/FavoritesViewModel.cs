using System;
using System.Threading.Tasks;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.User
{
	public class FavoritesViewModel : LoadableNetworkViewModel
	{
		#region Private fields and properties

		private readonly IStoreManager _storeManager;
		private readonly IProductManager _productManager;

		#endregion

		#region Constructors

		public FavoritesViewModel(IStoreManager storeManager, IProductManager productManager)
			:base()
		{
			this._productManager = productManager;
			this._storeManager = storeManager;
		}

		#endregion

		#region Properties

		public Action ShowFavoriteProductsAction;
		public Action ShowFavoriteStoresAction;

		private int _favoriteProductsCount;

		public int FavoriteProductsCount
		{ 
			get
			{
				return _favoriteProductsCount; 
			}
			set
			{
				_favoriteProductsCount = value; 
				RaisePropertyChanged(() => FavoriteProductsCount); 
				RaisePropertyChanged(() => FavoriteProductsCountLabel);
			}
		}

		private int _favoriteStoresCount;

		public int FavoriteStoresCount
		{ 
			get
			{
				return _favoriteStoresCount; 
			}
			set
			{
				_favoriteStoresCount = value; 
				RaisePropertyChanged(() => FavoriteStoresCount); 
				RaisePropertyChanged(() => FavoriteStoresCountLabel);
			}
		}

		public string FavoriteProductsCountLabel
		{
			get
			{
				if (this.FavoriteProductsCount > 0)
					return string.Format(AppResources.LabelListCount, this.FavoriteProductsCount);
				else
					return AppResources.LabelEmptyList;
			}
		}

		public string FavoriteStoresCountLabel
		{
			get
			{
				if (this.FavoriteStoresCount > 0)
					return string.Format(AppResources.LabelListCount, this.FavoriteStoresCount);
				else
					return AppResources.LabelEmptyList;
			}
		}

		private bool _areCountsLoaded;

		public bool AreCountsLoaded
		{ 
			get
			{
				return _areCountsLoaded; 
			}
			private set
			{
				_areCountsLoaded = value; 
				RaisePropertyChanged(() => AreCountsLoaded); 
			}
		}
		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showFavoriteProductsCommand;

		public System.Windows.Input.ICommand ShowFavoriteProductsCommand
		{
			get
			{
				_showFavoriteProductsCommand = _showFavoriteProductsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowFavoriteProductsAction.Fire();
				});
				return _showFavoriteProductsCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showFavoriteStoresCommand;

		public System.Windows.Input.ICommand ShowFavoriteStoresCommand
		{
			get
			{
				_showFavoriteStoresCommand = _showFavoriteStoresCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowFavoriteStoresAction.Fire();
				});
				return _showFavoriteStoresCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _loadFavoriteCountsCommand;

		public System.Windows.Input.ICommand LoadFavoriteCountsCommand
		{
			get
			{
				_loadFavoriteCountsCommand = _loadFavoriteCountsCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await loadCounts();
				});
				return _loadFavoriteCountsCommand;
			}
		}
			
		#endregion

		#region Utility methods

		private async Task loadCounts()
		{
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failureMessage = null;
			try
			{
				var productsCount = await _productManager.CountFavorite();
				var storesCount = await _storeManager.CountFavorite();
				this.FavoriteStoresCount = storesCount;
				this.FavoriteProductsCount = productsCount;
				this.AreCountsLoaded = true;
			}
			catch (OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				failureMessage = AppResources.MessageSubmissionTimeoutFailure;
			}
			catch (Exception ex)
			{
				Tools.Logger.Log("Error occured while loading data in Favorites page. {0}", ex.ToString(), LoggingLevel.Error);
				failureMessage = AppResources.MessageSubmissionUnknownFailure;
			}
			this.StopLoading(failureMessage);
		}

		#endregion
	}
}


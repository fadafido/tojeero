using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Logging;
using Tojeero.Core.Managers.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Common;

namespace Tojeero.Core.ViewModels.User
{
    public class FavoritesViewModel : BaseLoadableNetworkViewModel
    {
        #region Private fields and properties

        private readonly IStoreManager _storeManager;
        private readonly IProductManager _productManager;

        #endregion

        #region Constructors

        public FavoritesViewModel(IStoreManager storeManager, IProductManager productManager)
        {
            _productManager = productManager;
            _storeManager = storeManager;
        }

        #endregion


        #region Lifecycle management

        public override void OnAppearing()
        {
            base.OnAppearing();
            LoadFavoriteCountsCommand.Execute(null);

        }

        #endregion

        #region Properties

        public Action ShowFavoriteProductsAction;
        public Action ShowFavoriteStoresAction;

        private int _favoriteProductsCount;

        public int FavoriteProductsCount
        {
            get { return _favoriteProductsCount; }
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
            get { return _favoriteStoresCount; }
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
                if (FavoriteProductsCount > 0)
                    return string.Format(AppResources.LabelListCount, FavoriteProductsCount);
                return AppResources.LabelEmptyList;
            }
        }

        public string FavoriteStoresCountLabel
        {
            get
            {
                if (FavoriteStoresCount > 0)
                    return string.Format(AppResources.LabelListCount, FavoriteStoresCount);
                return AppResources.LabelEmptyList;
            }
        }

        private bool _areCountsLoaded;

        public bool AreCountsLoaded
        {
            get { return _areCountsLoaded; }
            private set
            {
                _areCountsLoaded = value;
                RaisePropertyChanged(() => AreCountsLoaded);
            }
        }

        #endregion

        #region Commands

        private MvxCommand _showFavoriteProductsCommand;

        public ICommand ShowFavoriteProductsCommand
        {
            get
            {
                _showFavoriteProductsCommand = _showFavoriteProductsCommand ??
                                               new MvxCommand(() => { ShowFavoriteProductsAction?.Invoke(); });
                return _showFavoriteProductsCommand;
            }
        }

        private MvxCommand _showFavoriteStoresCommand;

        public ICommand ShowFavoriteStoresCommand
        {
            get
            {
                _showFavoriteStoresCommand = _showFavoriteStoresCommand ??
                                             new MvxCommand(() => { ShowFavoriteStoresAction?.Invoke(); });
                return _showFavoriteStoresCommand;
            }
        }

        private MvxCommand _loadFavoriteCountsCommand;

        public ICommand LoadFavoriteCountsCommand
        {
            get
            {
                _loadFavoriteCountsCommand = _loadFavoriteCountsCommand ??
                                             new MvxCommand(async () => { await loadCounts(); });
                return _loadFavoriteCountsCommand;
            }
        }

        #endregion

        #region Utility methods

        private async Task loadCounts()
        {
            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            try
            {
                var productsCount = await _productManager.CountFavorite();
                var storesCount = await _storeManager.CountFavorite();
                FavoriteStoresCount = storesCount;
                FavoriteProductsCount = productsCount;
                AreCountsLoaded = true;
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                failureMessage = AppResources.MessageSubmissionTimeoutFailure;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log("Error occured while loading data in Favorites page. {0}", ex.ToString(),
                    LoggingLevel.Error);
                failureMessage = AppResources.MessageSubmissionUnknownFailure;
            }
            StopLoading(failureMessage);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Logging;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels.Product
{
    public class ProductDetailsViewModel : ProductViewModel
    {
        #region Private APIs and Fields

        private readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
        private IEnumerable<string> _detailImages;
        private IProduct _currentProduct;

        #endregion

        #region Constructors

        public ProductDetailsViewModel(IProduct product = null)
            : base(product)
        {
            ShouldSubscribeToSessionChange = true;
            PropertyChanged += propertyChanged;
        }

        #endregion

        #region Properties

        public Action<IStore> ShowStoreInfoPageAction { get; set; }
        public Action<IChatChannel> ShowChatPageAction { get; set; }

        private ContentMode _mode;

        public ContentMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                RaisePropertyChanged(() => Mode);
            }
        }

        private IList<string> _imageUrls;

        public IList<string> ImageUrls
        {
            get { return _imageUrls; }
            set
            {
                _imageUrls = value;
                RaisePropertyChanged(() => ImageUrls);
                CurrentImageUrl = ImageUrls.FirstOrDefault();
            }
        }

        private string _currentImageUrl;

        public string CurrentImageUrl
        {
            get { return _currentImageUrl; }
            set
            {
                _currentImageUrl = value;
                RaisePropertyChanged(() => CurrentImageUrl);
            }
        }

        public bool IsStoreDetailsVisible
        {
            get
            {
                return Mode == ContentMode.View && Product != null && Product.Store != null &&
                       !string.IsNullOrEmpty(Product.Store.Name);
            }
        }

        public override string StatusWarning
        {
            get
            {
                string warning = null;
                if (Mode == ContentMode.Edit && Product != null)
                {
                    if (Product.IsBlocked)
                    {
                        warning = AppResources.MessageStoreBlocked;
                    }
                    else
                    {
                        switch (Product.Status)
                        {
                            case ProductStatus.Pending:
                                warning = AppResources.MessageProductPending;
                                break;
                            case ProductStatus.Declined:
                            {
                                var reason = !string.IsNullOrEmpty(Product.DisapprovalReason)
                                    ? Product.DisapprovalReason
                                    : AppResources.TextUnknown;
                                warning = string.Format(AppResources.MessageProductDeclined, reason);
                            }
                                break;
                        }
                    }
                }
                return warning;
            }
        }

        #endregion

        #region Commands

        private MvxCommand _reloadCommand;

        public ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ??
                                 new MvxCommand(async () => { await reload(); }, () => !IsLoading && IsNetworkAvailable);
                return _reloadCommand;
            }
        }

        private MvxCommand _showStoreInfoPageCommand;

        public ICommand ShowStoreInfoPageCommand
        {
            get
            {
                _showStoreInfoPageCommand = _showStoreInfoPageCommand ??
                                            new MvxCommand(() => { ShowStoreInfoPageAction.Fire(Product.Store); },
                                                () => Product != null && Product.Store != null);
                return _showStoreInfoPageCommand;
            }
        }

        private MvxCommand _chatCommand;

        public ICommand ChatCommand
        {
            get
            {
                _chatCommand = _chatCommand ?? new MvxCommand(() =>
                {
                    var channel = getChannel();
                    ShowChatPageAction?.Invoke(channel);
                });
                return _chatCommand;
            }
        }

        #endregion

        #region Utility methods

        private async Task reload()
        {
            using (var writerLock = await _locker.WriterLockAsync())
            {
                if (Product != null)
                    await Product.LoadRelationships();
                await loadFavorite();
                await loadImages();
            }
        }

        private async Task loadImages()
        {
            if (Product == null)
                return;

            StartLoading(AppResources.MessageGeneralLoading);
            string failureMessage = null;
            try
            {
                var images = await Product.GetImages();
                if (images != null)
                {
                    _detailImages = images.Select(i => i.Url);
                }
                else
                {
                    _detailImages = null;
                }
                loadImageUrls();
            }
            catch (Exception ex)
            {
                failureMessage = handleException(ex);
            }
            StopLoading(failureMessage);
        }

        private string handleException(Exception exception)
        {
            try
            {
                throw exception;
            }
            catch (OperationCanceledException ex)
            {
                Tools.Logger.Log(ex, LoggingLevel.Warning);
                return AppResources.MessageLoadingTimeOut;
            }
            catch (Exception ex)
            {
                Tools.Logger.Log(ex, "Error occured while loading data in product details screen.", LoggingLevel.Error,
                    true);
                return AppResources.MessageLoadingFailed;
            }
        }

        private void loadImageUrls()
        {
            IList<string> imageUrls = new List<string>();
            if (Product != null && !string.IsNullOrEmpty(Product.ImageUrl))
            {
                imageUrls.Add(Product.ImageUrl);
            }
            if (_detailImages != null)
            {
                imageUrls.AddRange(_detailImages);
            }

            ImageUrls = imageUrls;
        }

        protected override void propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.propertyChanged(sender, e);
            if (e.PropertyName == ProductProperty || e.PropertyName == "")
            {
                if (_currentProduct != Product)
                {
                    _currentProduct = Product;
                    loadImageUrls();
                }
            }

            if (e.PropertyName == "Status" || e.PropertyName == "Mode" || e.PropertyName == "")
            {
                RaisePropertyChanged(() => StatusWarning);
                RaisePropertyChanged(() => WarningColor);
            }

            if (e.PropertyName == "Product" || e.PropertyName == "Store" || e.PropertyName == "Name" ||
                e.PropertyName == "Mode" || e.PropertyName == "")
            {
                RaisePropertyChanged(() => IsStoreDetailsVisible);
            }
        }

        private IChatChannel getChannel()
        {
            var channel = new ChatChannel
            {
                ChannelID = "test_channel",
                RecipientID = Product.Store.OwnerID,
                RecipientProfilePictureUrl = Product.Store.ImageUrl,
                SenderID = _authService.CurrentUser.ID,
                SenderProfilePictureUrl = _authService.CurrentUser.ProfilePictureUrl
            };
            return channel;
        }

        #endregion
    }
}
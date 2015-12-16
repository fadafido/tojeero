using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core.ViewModels
{
	public class ProductDetailsViewModel : ProductViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
		private IEnumerable<string> _detailImages;
		private IProduct _currentProduct;

		#endregion

		#region Constructors

		public ProductDetailsViewModel(IProduct product = null)
			: base(product)
		{
			this.ShouldSubscribeToSessionChange = true;
			this.PropertyChanged += propertyChanged;
		}

		#endregion

		#region Properties

		public Action<IStore> ShowStoreInfoPageAction;

		private ContentMode _mode;

		public ContentMode Mode
		{ 
			get
			{
				return _mode; 
			}
			set
			{
				_mode = value; 
				RaisePropertyChanged(() => Mode); 
			}
		}

		private IList<string> _imageUrls;

		public IList<string> ImageUrls
		{ 
			get
			{
				return _imageUrls; 
			}
			set
			{
				_imageUrls = value; 
				RaisePropertyChanged(() => ImageUrls); 
				this.CurrentImageUrl = this.ImageUrls.FirstOrDefault();
			}
		}

		private string _currentImageUrl;

		public string CurrentImageUrl
		{ 
			get
			{
				return _currentImageUrl; 
			}
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
				return this.Mode == ContentMode.View && this.Product != null && this.Product.Store != null && !string.IsNullOrEmpty(this.Product.Store.Name);
			}
		}

		public override string StatusWarning
		{
			get
			{
				string warning = null;
				if (this.Mode == ContentMode.Edit && this.Product != null)
				{
					if (this.Product.IsBlocked)
					{
						warning = AppResources.MessageStoreBlocked;
					}
					else
					{
						switch (this.Product.Status)
						{
							case ProductStatus.Pending:
								warning = AppResources.MessageProductPending;
								break;
							case ProductStatus.Declined:
								{
									string reason = !string.IsNullOrEmpty(this.Product.DisapprovalReason) ? this.Product.DisapprovalReason : AppResources.TextUnknown;
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

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => {
					await reload();
				}, () => !IsLoading && IsNetworkAvailable);
				return _reloadCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _showStoreInfoPageCommand;

		public System.Windows.Input.ICommand ShowStoreInfoPageCommand
		{
			get
			{
				_showStoreInfoPageCommand = _showStoreInfoPageCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => {
					ShowStoreInfoPageAction.Fire(this.Product.Store);
				}, () => this.Product != null && this.Product.Store != null);
				return _showStoreInfoPageCommand;
			}
		}

		#endregion

		#region Utility methods

		private async Task reload()
		{
			using (var writerLock = await _locker.WriterLockAsync())
			{
				if (this.Product != null)
					await this.Product.LoadRelationships();
				await loadFavorite();
				await loadImages();
			}
		}

		private async Task loadImages()
		{
			if (this.Product == null)
				return;
			
			this.StartLoading(AppResources.MessageGeneralLoading);
			string failureMessage = null;
			try
			{
				var images = await this.Product.GetImages();
				if(images != null)
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
				Tools.Logger.Log(ex, "Error occured while loading data in product details screen.", LoggingLevel.Error, true);
				return AppResources.MessageLoadingFailed;
			}
		}

		private void loadImageUrls()
		{
			IList<string> imageUrls = new List<string>();
			if (this.Product != null && !string.IsNullOrEmpty(this.Product.ImageUrl))
			{
				imageUrls.Add(this.Product.ImageUrl);
			}
			if (_detailImages != null)
			{
				imageUrls.AddRange(_detailImages);
			}

			this.ImageUrls = imageUrls;
		}

		protected override void propertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.propertyChanged(sender, e);
			if (e.PropertyName == ProductProperty || e.PropertyName == "")
			{
				if (_currentProduct != this.Product)
				{
					_currentProduct = this.Product;
					loadImageUrls();
				}
			}

			if (e.PropertyName == "Status" || e.PropertyName == "Mode" || e.PropertyName == "")
			{
				RaisePropertyChanged(() => StatusWarning);
			}		if (e.PropertyName == "Product" || e.PropertyName == "Store" || e.PropertyName == "Name" || e.PropertyName == "Mode" || e.PropertyName == "")
			{
				RaisePropertyChanged(() => IsStoreDetailsVisible);
			}
		}

		#endregion
	}
}


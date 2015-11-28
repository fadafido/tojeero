using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core.ViewModels
{
	public class ProductDetailsViewModel : ProductViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public ProductDetailsViewModel(IProduct product = null)
			: base(product)
		{
			this.ShouldSubscribeToSessionChange = true;
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
				await loadFavorite();
			}
		}

		#endregion
	}
}


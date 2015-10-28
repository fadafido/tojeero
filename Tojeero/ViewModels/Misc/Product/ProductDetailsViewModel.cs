using System;
using System.Threading.Tasks;
using Nito.AsyncEx;

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

		#endregion

		#region Utility methods

		private async Task reload()
		{
			using (var writerLock = await _locker.WriterLockAsync())
			{
				if (!IsFavoriteLoaded && CanExecuteLoadFavoriteCommand)
					await loadFavorite();
			}
		}

		#endregion
	}
}


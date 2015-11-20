using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.CrossCore;
using Tojeero.Core.Messages;

namespace Tojeero.Core.ViewModels
{
	public class StoreDetailsViewModel : StoreViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public StoreDetailsViewModel(IStore store = null)
			: base(store)
		{
			this.ShouldSubscribeToSessionChange = true;
			var messenger = Mvx.Resolve<IMvxMessenger>();
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
				await loadFavorite();
			}
		}

		#endregion
	}
}


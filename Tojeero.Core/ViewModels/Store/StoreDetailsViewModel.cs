using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Nito.AsyncEx;
using Tojeero.Core.Model;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Store
{
	public class StoreDetailsViewModel : StoreViewModel
	{
		#region Private APIs and Fields

		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public StoreDetailsViewModel(IStore store = null, ContentMode mode = ContentMode.View)
			: base(store)
		{
			this.ShouldSubscribeToSessionChange = true;
			var messenger = Mvx.Resolve<IMvxMessenger>();
		}

        #endregion

        #region Properties

        public Action<IChatChannel> ShowChatPageAction { get; set; }

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
				if (this.Store != null)
					await this.Store.LoadRelationships();
				await loadFavorite();
			}
		}

        private IChatChannel getChannel()
        {
            var channel = new ChatChannel()
            {
                ChannelID = "test_channel",
                RecipientID = Store.OwnerID,
                RecipientProfilePictureUrl = Store.ImageUrl,
                SenderID = _authService.CurrentUser.ID,
                SenderProfilePictureUrl = _authService.CurrentUser.ProfilePictureUrl
            };
            return channel;
        }

        #endregion
    }
}


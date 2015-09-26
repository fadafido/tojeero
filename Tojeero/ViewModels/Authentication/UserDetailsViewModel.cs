using System;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Services;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Threading.Tasks;
using Tojeero.Core.Toolbox;
using System.Threading;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;

namespace Tojeero.Core.ViewModels
{
	public class UserDetailsViewModel : BaseUserDetailsViewModel
	{
		#region Private Fields and Properties

		CancellationTokenSource _tokenSource;
		CancellationToken _token;

		#endregion

		#region Constructors

		public UserDetailsViewModel(IAuthenticationService authService, IMvxMessenger messenger)
			: base(authService, messenger)
		{
		}

		#endregion

		#region Properties

		public event EventHandler<EventArgs> Close;

		public string Title
		{
			get
			{
				return "User Details";
			}
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _submitCommand;
		public System.Windows.Input.ICommand SubmitCommand
		{
			get
			{
				_submitCommand = _submitCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(async () => await submit(), () => !IsLoading);
				return _submitCommand;
			}
		}

		private Cirrious.MvvmCross.ViewModels.MvxCommand _cancelCommand;
		public System.Windows.Input.ICommand CancelCommand
		{
			get
			{
				_cancelCommand = _cancelCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(() => cancel());
				return _cancelCommand;
			}
		}

		#endregion

		#region Utility Methods

		private async Task submit()
		{
			this.IsLoading = true;
			using (_tokenSource = new CancellationTokenSource())
			{
				_token = _tokenSource.Token;
				try
				{
					await Task.Delay(5000, _token);
				}
				catch (Exception ex)
				{
					Mvx.Trace(MvxTraceLevel.Error, "Error occured while saving user details. {0}", ex.ToString());
				}

				this.IsLoading = false;
				this.Close.Fire(this, new EventArgs());
			}
		}

		private void cancel()
		{
			//If we have a token source it mean that submit process has already started
			//so we need to cancel it and the view will be closed inside submit method
			if (_tokenSource != null)
			{
				_tokenSource.Cancel();
			}
			//Otherwise just close the view
			else
			{
				this.Close.Fire(this, new EventArgs());
			}
		}

		#endregion

	}
}


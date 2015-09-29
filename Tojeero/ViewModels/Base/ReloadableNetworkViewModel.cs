using System;

namespace Tojeero.Core.ViewModels
{
	public abstract class ReloadableNetworkViewModel : LoadableNetworkViewModel
	{
		#region Constructors

		public ReloadableNetworkViewModel()
			: base()
		{
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public virtual System.Windows.Input.ICommand ReloadCommand
		{
			get
			{
				_reloadCommand = _reloadCommand ?? new Cirrious.MvvmCross.ViewModels.MvxCommand(DoReloadCommand);
				return _reloadCommand;
			}
		}

		protected abstract void DoReloadCommand();

		#endregion
	}
}


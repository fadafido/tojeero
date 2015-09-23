using System;

namespace Tojeero.Core.ViewModels
{
	public abstract class ReloadableViewModel : LoadableViewModel, IReloadableViewModel
	{
		#region Constructors

		public ReloadableViewModel()
			: base()
		{
		}

		#endregion

		#region Commands

		private Cirrious.MvvmCross.ViewModels.MvxCommand _reloadCommand;

		public System.Windows.Input.ICommand ReloadCommand
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


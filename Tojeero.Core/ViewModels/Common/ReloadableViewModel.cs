using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
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


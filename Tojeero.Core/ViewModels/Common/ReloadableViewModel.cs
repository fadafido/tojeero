using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.ViewModels.Contracts;

namespace Tojeero.Core.ViewModels.Common
{
    public abstract class ReloadableViewModel : LoadableViewModel, IReloadableViewModel
    {
        #region Constructors

        #endregion

        #region Commands

        private MvxCommand _reloadCommand;

        public virtual ICommand ReloadCommand
        {
            get
            {
                _reloadCommand = _reloadCommand ?? new MvxCommand(DoReloadCommand);
                return _reloadCommand;
            }
        }

        protected abstract void DoReloadCommand();

        #endregion
    }
}
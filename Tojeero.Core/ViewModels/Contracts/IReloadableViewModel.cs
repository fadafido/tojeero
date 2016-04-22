using System.Windows.Input;

namespace Tojeero.Core.ViewModels.Contracts
{
    public interface IReloadableViewModel : ILoadableViewModel
    {
        ICommand ReloadCommand { get; }
    }
}
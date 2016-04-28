using System;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels.Contracts
{
    public interface ISearchViewModel : IBaseViewModel
    {
        event EventHandler<EventArgs> LoadingNextPageFinished;
        event EventHandler<EventArgs> ReloadFinished;
        string SearchQuery { get; set; }
        ICommand ItemSelectedCommand { get; }
    }
}
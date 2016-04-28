using System;
using System.Windows.Input;
using Tojeero.Core.Contracts;

namespace Tojeero.Core.ViewModels.Contracts
{
    public interface ICollectionViewModel : IBaseViewModel
    {
        event EventHandler<EventArgs> LoadingNextPageFinished;
        event EventHandler<EventArgs> ReloadFinished;
        int Count { get; }
        bool IsLoadingInitialData { get; }
        bool IsLoadingNextPage { get; }
        ICommand TryAgainCommand { get; }
        ICommand LoadFirstPageCommand { get; }
        ICommand RefetchCommand { get; }
        ICommand LoadNextPageCommand { get; }
        bool CanExecuteLoadNextPageCommand { get; }
        ICommand ReloadCommand { get; }
        bool CanExecuteReloadCommand { get; }
        ICommand ItemSelectedCommand { get; }
    }

    public interface ICollectionViewModel<T> : ICollectionViewModel
    {
        IModelEntityCollection<T> Collection { get; set; }
    }
}
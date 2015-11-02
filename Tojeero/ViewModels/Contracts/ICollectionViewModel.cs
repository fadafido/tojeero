using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Tojeero.Core.Toolbox;
using Tojeero.Forms.Resources;
using Nito.AsyncEx;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
{
	public interface ICollectionViewModel
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
	}

	public interface ICollectionViewModel<T> : ICollectionViewModel
	{
		IModelEntityCollection<T> Collection { get; set; }
	}
}

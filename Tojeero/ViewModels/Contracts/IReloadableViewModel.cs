using System;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
{
	public interface IReloadableViewModel : ILoadableViewModel
	{
		ICommand ReloadCommand { get; }
	}
}


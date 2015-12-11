using System;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
{
	public interface ISocialViewModel : IUserViewModel
	{
		IFavorite Favorite { get; set; }
		ICommand LoadFavoriteCommand { get; }
		bool CanExecuteLoadFavoriteCommand { get; }
		ICommand ToggleFavoriteCommand { get; }
		bool CanExecuteToggleFavoriteCommand { get; }
	}
}


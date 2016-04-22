using System.Windows.Input;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Contracts
{
    public interface ISocialViewModel : IUserViewModel
    {
        IFavorite Favorite { get; set; }
        bool IsFavoriteToggleVisible { get; }
        ICommand LoadFavoriteCommand { get; }
        bool CanExecuteLoadFavoriteCommand { get; }
        ICommand ToggleFavoriteCommand { get; }
        bool CanExecuteToggleFavoriteCommand { get; }
    }
}
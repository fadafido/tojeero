using System.ComponentModel;

namespace Tojeero.Core.Model.Contracts
{
    public interface IFavorite : INotifyPropertyChanged
    {
        string ObjectID { get; set; }
        bool IsFavorite { get; set; }
    }
}
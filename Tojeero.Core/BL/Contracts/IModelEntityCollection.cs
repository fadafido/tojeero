using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Tojeero.Core.Contracts
{
    public interface IModelEntityCollection : INotifyCollectionChanged, INotifyPropertyChanged
    {
        Task FetchNextPageAsync();
    }

    public interface IModelEntityCollection<T> : IModelEntityCollection, ICollection<T>
    {
    }
}
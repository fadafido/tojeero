using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core
{
	public interface IModelQuery<T> where T : IModelEntity
	{
		Task<IEnumerable<T>> Fetch(int pageSize = - 1, int offset = -1);
		Comparison<T> Comparer { get; }
		Task ClearCache();
	}

	public interface IModelEntityCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		Task FetchNextPageAsync();
	}
}


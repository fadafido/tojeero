using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core
{
	public delegate Task<IEnumerable<T>> QueryDelegate<T>(int pageSize, int offset, CancellationToken token) where T : IModelEntity;

	public interface IModelEntityCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		Task FetchNextPageAsync();
		Task FetchNextPageAsync(CancellationToken token);
	}
}


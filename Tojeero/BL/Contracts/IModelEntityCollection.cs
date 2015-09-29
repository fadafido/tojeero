using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core
{
	public interface IModelEntityCollection<EntityType> : ICollection<EntityType>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		Task FetchNextPageAsync();
		Task FetchNextPageAsync(CancellationToken token);
	}
}


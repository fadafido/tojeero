using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core
{

	public interface IModelEntityCollection : INotifyCollectionChanged, INotifyPropertyChanged
	{
		Task FetchNextPageAsync();
	}

	public interface IModelEntityCollection<T> : IModelEntityCollection, ICollection<T>
	{
		
	}
}


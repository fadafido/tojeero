﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Tojeero.Core.Toolbox;
using Nito.AsyncEx;

namespace Tojeero.Core
{
	public class ModelEntityCollection<T> : ObservableCollection<T>, IModelEntityCollection<T>
	{
		#region Private fields and properties

		private readonly int _pageSize;
		IModelQuery<T> _query;
		private int _previousCount = 0;
		private bool _isAllDataLoaded = false;
		private AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

		#endregion

		#region Constructors

		public ModelEntityCollection(IModelQuery<T> query, int pageSize = -1)
		{
			this._pageSize = pageSize;
			_query = query;
		}

		#endregion

		#region IModelEntityCollection implementation

		public async Task FetchNextPageAsync()
		{
			using (var writerLock = await _locker.WriterLockAsync())
			{
				try
				{
					if (_isAllDataLoaded)
						return;
					var result = await _query.Fetch(_pageSize, this.Count);	
					if(_query.Comparer != null)
						this.InsertSorted(result, _query.Comparer);
					else
						this.AddRange(result);
					if (this.Count - _previousCount < _pageSize)
					{
						_isAllDataLoaded = true;
					}
					_previousCount = this.Count;
				}
				catch (OperationCanceledException ex)
				{
					Tools.Logger.Log(ex, LoggingLevel.Warning);
					throw ex;
				}
				catch (Exception ex)
				{
					Tools.Logger.Log(ex, new Dictionary<string, string>
						{
					["Description" ] ="Error occured while fetching data using ModelEntityCollection.",
					["Model entity" ] =typeof(T).FullName,
					["Page size" ] =this._pageSize.ToString(),
					["Current offset" ] =this.Count.ToString(),
						}, LoggingLevel.Error, true);
					throw ex;
				}
			}
		}


		#endregion
	}
}


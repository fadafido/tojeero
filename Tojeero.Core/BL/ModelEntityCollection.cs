using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Tojeero.Core.Contracts;
using Tojeero.Core.Logging;
using Tojeero.Core.Model.Contracts;
using Tojeero.Core.Toolbox;

namespace Tojeero.Core
{
    public class ModelEntityCollection<T> : ObservableCollection<T>, IModelEntityCollection<T>
    {
        #region Private fields and properties

        private readonly int _pageSize;
        readonly IModelQuery<T> _query;
        private int _previousCount;
        private bool _isAllDataLoaded;
        private readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

        #endregion

        #region Constructors

        public ModelEntityCollection(IModelQuery<T> query, int pageSize = -1)
        {
            _pageSize = pageSize;
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
                    var result = await _query.Fetch(_pageSize, Count);
                    if (_query.Comparer != null)
                        this.InsertSorted(result, _query.Comparer);
                    else
                        this.AddRange(result);
                    if (Count - _previousCount < _pageSize)
                    {
                        _isAllDataLoaded = true;
                    }
                    _previousCount = Count;
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
                        ["Description"] = "Error occured while fetching data using ModelEntityCollection.",
                        ["Model entity"] = typeof (T).FullName,
                        ["Page size"] = _pageSize.ToString(),
                        ["Current offset"] = Count.ToString()
                    }, LoggingLevel.Error, true);
                    throw ex;
                }
            }
        }

        #endregion
    }
}
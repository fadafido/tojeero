using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Tojeero.Core
{
	public class ModelEntityCollection<T> : ObservableCollection<T>, IModelEntityCollection<T>
		where T : IModelEntity
	{
		#region Private fields and properties

		private readonly int _pageSize;
		IModelQuery<T> _query;

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
			try
			{
				var result = await _query.Fetch(_pageSize, this.Count);
				this.InsertSorted(result, _query.Comparer);
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
				throw ex;
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, new Dictionary<string, string>{
					["Description"]="Error occured while fetching data using ModelEntityCollection.",
					["Model entity"]=typeof(T).FullName,
					["Page size"]=this._pageSize.ToString(),
					["Current offset"]=this.Count.ToString(),
				}, LoggingLevel.Error, true);
				throw ex;
			}
		}


		#endregion
	}
}


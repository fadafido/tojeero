using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace Tojeero.Core
{
	public class ModelEntityCollection<T> : ObservableCollection<T>, IModelEntityCollection<T>
		where T : IModelEntity
	{
		#region Private fields and properties

		private readonly int _pageSize;
		QueryDelegate<T> _query;

		#endregion

		#region Constructors

		public ModelEntityCollection(QueryDelegate<T> query, int pageSize = -1)
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
				var result = await _query(_pageSize, this.Count);
				foreach(var item in result)
					this.Add(item);
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


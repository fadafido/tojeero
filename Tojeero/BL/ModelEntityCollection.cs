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

		public Task FetchNextPageAsync()
		{
			return FetchNextPageAsync(CancellationToken.None);
		}

		public async Task FetchNextPageAsync(CancellationToken token)
		{
			try
			{
				var result = await _query(this.Count, _pageSize, token);
				foreach(var item in result)
					this.Add(item);
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Error);
			}
		}


		#endregion
	}
}


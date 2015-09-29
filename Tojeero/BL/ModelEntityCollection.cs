using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;

namespace Tojeero.Core
{
	public class ModelEntityCollection<EntityType> : ObservableCollection<EntityType>, IModelEntityCollection<EntityType>
		where EntityType : IModelEntity
	{
		#region Private fields and properties

		private readonly IModelEntityManager<EntityType> _manager;
		private readonly int _pageSize;
		#endregion

		#region Constructors

		public ModelEntityCollection(IModelEntityManager<EntityType> manager, int pageSize = -1)
		{
			this._manager = manager;
			this._pageSize = pageSize;
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
				var result = await _manager.FetchAsync(this.Count, _pageSize, token);
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


using System;

namespace Tojeero.Core.ViewModels
{
	public class StoresViewModel : BaseCollectionViewModel<IStore>
	{
		#region Private fields and properties

		private readonly IStoreManager _manager;

		#endregion

		#region Constructors

		public StoresViewModel(IStoreManager manager)
			: base(manager)
		{
			_manager = manager;
		}

		#endregion
	}
}


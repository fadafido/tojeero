using System;

namespace Tojeero.Core.ViewModels
{
	public class BaseCollectionViewModel<T> : ReloadableNetworkViewModel
	{
		#region Constructors

		public BaseCollectionViewModel()
			: base()
		{
		}

		#endregion

		#region abstract members of ReloadableNetworkViewModel

		protected override void DoReloadCommand()
		{
			
		}

		#endregion
	}
}


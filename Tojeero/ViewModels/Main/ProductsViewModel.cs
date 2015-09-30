using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseCollectionViewModel<IProduct>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;

		#endregion

		#region Properties

		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager)
			: base(manager.FetchProducts, 20)
		{
			_manager = manager;
			LoadNextPageCommand.Execute(null);
		}

		#endregion
	}
}


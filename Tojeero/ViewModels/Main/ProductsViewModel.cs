using System;

namespace Tojeero.Core.ViewModels
{
	public class ProductsViewModel : BaseCollectionViewModel<IProduct>
	{
		#region Private fields and properties

		private readonly IProductManager _manager;

		#endregion

		#region Constructors

		public ProductsViewModel(IProductManager manager)
			: base(manager.FetchProducts)
		{
			_manager = manager;
			LoadNextPageCommand.Execute(null);


		}

		#endregion
	}
}


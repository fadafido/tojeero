using System.Collections.ObjectModel;
using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Contracts
{
	public interface ISaveProductViewModel
	{
		IProduct CurrentProduct { get; set; }

		IStore Store { get; set; }

		bool IsNew { get; }

		IImageViewModel MainImage { get; set; }

		ObservableCollection<IImageViewModel> Images { get; set; }

		string Name { get; set; }

		double Price { get; set; }

		IProductCategory Category { get; set; }

		IProductSubcategory Subcategory { get; set; }

		ObservableCollection<string> Tags { get; set; }

		string Description { get; set; }

		bool Visible { get; set; }

		bool HasChanged { get; }
	}
}


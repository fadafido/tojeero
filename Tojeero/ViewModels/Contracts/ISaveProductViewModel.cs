using System;
using Xamarin.Forms;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Tojeero.Core.ViewModels
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

		string Description { get; set; }

		bool HasChanged { get; }
	}
}


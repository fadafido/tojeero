using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
{
	public interface ISaveStoreViewModel
	{
		string Title { get; }

		IStore CurrentStore { get; set; }

		IImageViewModel MainImage { get; set; }

		string Name { get; set; }

		string Description { get; set; }

		string DeliveryNotes { get; set; }

		IStoreCategory Category { get; set; }

		ICity City { get; set; }

		ICountry Country { get; set; }

		bool HasChanged { get; }

		ICommand SaveCommand { get; }

		ICommand PickMainImageCommand { get; }

		ICommand RemoveMainImageCommand { get; }

		bool CanExecuteRemoveMainImageCommand { get; }
	}
}


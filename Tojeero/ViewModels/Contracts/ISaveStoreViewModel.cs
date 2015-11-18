using System;
using Xamarin.Forms;
using System.Windows.Input;

namespace Tojeero.Core.ViewModels
{
	public interface ISaveStoreViewModel
	{
		IStore CurrentStore { get; set; }

		bool IsNew { get; }

		IImageViewModel MainImage { get; set; }

		string Name { get; set; }

		string Description { get; set; }

		string DeliveryNotes { get; set; }

		IStoreCategory Category { get; set; }

		ICity City { get; set; }

		ICountry Country { get; set; }

		bool HasChanged { get; }
	}
}


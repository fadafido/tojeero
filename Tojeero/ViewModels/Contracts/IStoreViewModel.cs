using System;
using Xamarin.Forms;

namespace Tojeero.Core.ViewModels
{
	public interface IStoreViewModel
	{
		IStore CurrentStore { get; set; }

		IImageViewModel MainImage { get; set; }

		string Name { get; set; }

		string Description { get; set; }

		string DeliveryNotes { get; set; }

		IStoreCategory Category { get; }

		ICity City { get; }

		ICountry Country { get; }

		bool HasChanged();
	}
}


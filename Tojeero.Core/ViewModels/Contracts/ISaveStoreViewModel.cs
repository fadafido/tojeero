using Tojeero.Core.Model.Contracts;

namespace Tojeero.Core.ViewModels.Contracts
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


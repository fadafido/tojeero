using System;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IUser : IModelEntity
	{
		string FirstName { get; set; }

		string LastName { get; set; }

		string FullName { get; }

		string UserName { get; set; }

		string Email { get; set; }

		string ProfilePictureUrl { get; set; }

		string CityId { get; set; }

		ICity City { get; }

		string CountryId { get; set; }

		ICountry Country { get; }

		IStore DefaultStore { get; }

		string Mobile { get; set; }

		bool IsProfileSubmitted { get; set; }

		Task<bool> IsProductFavorite(string productID);

		Task AddProductToFavorites(string productID);

		Task RemoveProductFromFavorites(string productID);

		Task<bool> IsStoreFavorite(string storeID);

		Task AddStoreToFavorites(string storeID);

		Task RemoveStoreFromFavorites(string storeID);

		Task LoadDefaultStore();
	}
}


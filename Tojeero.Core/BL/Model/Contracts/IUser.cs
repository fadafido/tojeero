using System.Threading.Tasks;

namespace Tojeero.Core.Model.Contracts
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

        Task<IFavorite> GetProductFavorite(string productID);

        Task<IFavorite> AddProductToFavorites(string productID);

        Task RemoveProductFromFavorites(string productID);

        Task<IFavorite> GetStoreFavorite(string storeID);

        Task<IFavorite> AddStoreToFavorites(string storeID);

        Task RemoveStoreFromFavorites(string storeID);

        Task LoadDefaultStore();
    }
}
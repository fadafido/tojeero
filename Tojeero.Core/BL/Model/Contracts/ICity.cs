namespace Tojeero.Core.Model.Contracts
{
    public interface ICity : IUniqueEntity
    {
        string Name { get; }
        string Name_en { get; }
        string Name_ar { get; }
        string CountryId { get; }
        ICountry Country { get; }
    }
}
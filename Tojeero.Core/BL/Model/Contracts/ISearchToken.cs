namespace Tojeero.Core.Model.Contracts
{
    public interface ISearchToken : IUniqueEntity
    {
        string EntityID { get; set; }
        string EntityType { get; set; }
        string Token { get; set; }
    }
}
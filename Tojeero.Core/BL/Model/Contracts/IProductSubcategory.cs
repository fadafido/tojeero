namespace Tojeero.Core.Model.Contracts
{
	public interface IProductSubcategory : IModelEntity
	{
		string CategoryID { get; set; }
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
	}
}

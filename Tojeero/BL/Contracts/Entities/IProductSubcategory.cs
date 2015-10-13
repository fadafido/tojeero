using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IProductSubcategory : IModelEntity
	{
		string CategoryID { get; set; }
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
	}
}

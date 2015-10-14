using System;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Tojeero.Core
{
	public interface IProductCategory : IModelEntity
	{
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
	}
}

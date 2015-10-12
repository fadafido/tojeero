using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IProductCategory : IModelEntity
	{
		string Name { get; set; }
	}
}

using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IStoreCategory : IModelEntity
	{
		string Name { get; }
		string Name_en { get; }
		string Name_ar { get; }
	}
}

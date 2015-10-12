using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IStoreCategory : IModelEntity
	{
		string Name { get; set; }
	}
}

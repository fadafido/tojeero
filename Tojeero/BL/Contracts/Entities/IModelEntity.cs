using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IModelEntity : IUniqueEntity, IMvxNotifyPropertyChanged
	{
		string ID { get; set; }
	}
}


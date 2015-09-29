using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public class BaseModelEntity : MvxNotifyPropertyChanged, IModelEntity
	{
		public BaseModelEntity()
		{
		}

		public string ID
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}


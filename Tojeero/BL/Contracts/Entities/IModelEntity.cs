﻿using System;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.Core
{
	public interface IModelEntity : IMvxNotifyPropertyChanged
	{
		string ID { get; set; }
	}
}


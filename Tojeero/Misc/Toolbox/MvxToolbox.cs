﻿using System;
using System.Linq;
using System.Reflection;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;
using Cirrious.MvvmCross.Platform;
using System.Collections.Generic;
using Cirrious.MvvmCross;
using Tojeero.Core;

namespace Tojeero.Forms.Toolbox
{
	public static class MvxToolbox
	{
		public static T LoadViewModel<T>(object parameterValuesObject = null) where T : IMvxViewModel
		{
			IDictionary<string, string> data = null;
			if (parameterValuesObject != null)
				data = parameterValuesObject.ToSimplePropertyDictionary();
			var request = new MvxViewModelRequest<T>(new MvxBundle(data), null, new MvxRequestedBy());
			return LoadViewModel<T>(request);
		}

		public static T LoadViewModel<T>(MvxViewModelRequest request) where T : IMvxViewModel
		{
			var viewModelLoader = Mvx.Resolve<IMvxViewModelLoader>();
			var viewModel = viewModelLoader.LoadViewModel(request, null);
			if (viewModel == null)
			{
				Tools.Logger.Log("Could not find view model of type {0}.", typeof(T));
				return default(T);
			}
			return (T)viewModel;
		}
	}
}


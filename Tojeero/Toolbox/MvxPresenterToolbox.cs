using System;
using System.Linq;
using System.Reflection;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;

namespace Tojeero.Forms.Toolbox
{
	public static class MvxPresenterToolbox
	{
		public static IMvxViewModel LoadViewModel(MvxViewModelRequest request)
		{
			var viewModelLoader = Mvx.Resolve<IMvxViewModelLoader>();
			var viewModel = viewModelLoader.LoadViewModel(request, null);
			return viewModel;
		}

		public static T CreatePage<T>(MvxViewModelRequest request)
			where T : class
		{
			var viewType = ResolveViewType(request.ViewModelType);

			if (viewType == null)
			{
				Mvx.Trace("Page not found for {0}",
					request.ViewModelType.Name);
				return null;
			}

			var page = Activator.CreateInstance(viewType) as T;
			if (page == null)
			{
				Mvx.Error("Failed to create ContentPage {0}", viewType.Name);
			}
			return page;
		}

		private static Type ResolveViewType(Type viewModelType)
		{
			var viewName = viewModelType.Name.Replace("ViewModel", "Page");
			var assemblyName = new AssemblyName("Tojeero.Forms");
			var assembly = Assembly.Load(assemblyName);
			if (assembly == null)
				return null;
			var type = assembly.CreatableTypes().FirstOrDefault(t => t.Name == viewName);

			return type;
		}
	}
}


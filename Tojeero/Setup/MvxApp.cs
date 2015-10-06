using Cirrious.CrossCore.IoC;
using Tojeero.Core.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross;
using Tojeero.Core;
using System;

namespace Tojeero
{
	
    public class MvxApp : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
		private class EmptyStart : Cirrious.MvvmCross.ViewModels.IMvxAppStart
		{
			#region IMvxAppStart implementation
			public void Start(object hint = null)
			{
				
			}
			#endregion
		}

        public override void Initialize()
        {
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			CreatableTypes()
				.EndingWith("Manager")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			CreatableTypes()
				.EndingWith("ViewModel")
				.AsTypes()
				.RegisterAsDynamic();

			Mvx.LazyConstructAndRegisterSingleton<ILogger, Logger>();
			RegisterAppStart(new EmptyStart());

			try
			{
				Mvx.Resolve<ICacheRepository>().Initialize().Wait();
				Mvx.Resolve<ICacheRepository>().Clear().Wait();
			}
			catch(OperationCanceledException ex)
			{
				Tools.Logger.Log(ex, LoggingLevel.Warning);
			}
			catch(Exception ex)
			{
				Tools.Logger.Log(ex, "Error occurred while clearing local cache before application launch.", LoggingLevel.Error, true);
			}
        }
    }
}
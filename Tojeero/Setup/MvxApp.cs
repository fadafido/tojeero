using Cirrious.CrossCore.IoC;
using Tojeero.Core.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross;
using Tojeero.Core;
using System;
using System.Globalization;

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
			//We need to set thread culture to english because if the device culture will be arabic
			//when sending requests to parse.com app will use arabic calender, which cause app crash.
			//This will insure that all requests sent to Parse.com will have english culture and thus Gregorian calendar.
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
			RegisterAppStart(new EmptyStart());
        }
    }
}
using System.Globalization;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core.Logging;

namespace Tojeero.Core
{
    public class MvxApp : MvxApplication
    {
        private class EmptyStart : IMvxAppStart
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

            CreatableTypes()
                .EndingWith("Repository")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.LazyConstructAndRegisterSingleton<ILogger, Logger>();
            //We need to set thread culture to english because if the device culture will be arabic
            //when sending requests to parse.com app will use arabic calender, which cause app crash.
            //This will insure that all requests sent to Parse.com will have english culture and thus Gregorian calendar.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");
            RegisterAppStart(new EmptyStart());
        }
    }
}
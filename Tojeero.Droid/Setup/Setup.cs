using Android.Content;
using Cirrious.CrossCore;
using Cirrious.CrossCore.IoC;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Droid.Platform;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Core;
using XLabs.Platform.Services.Media;

namespace Tojeero.Droid
{
    public class Setup : MvxAndroidSetup
    {
        public Setup(Context applicationContext) : base(applicationContext)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new MvxApp();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeFirstChance()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.LazyConstructAndRegisterSingleton<IMediaPicker, MediaPicker>();
        }
    }
}
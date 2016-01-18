using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using UIKit;
using Tojeero.Core;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.CrossCore.IoC;
using Cirrious.CrossCore;
using XLabs.Platform.Services.Media;

namespace Tojeero.iOS
{
	public class Setup : MvxTouchSetup
	{
		public Setup(IMvxApplicationDelegate applicationDelegate, UIWindow window)
			: base(applicationDelegate, window)
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

			CreatableTypes()
				.EndingWith("Manager")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			CreatableTypes()
				.EndingWith("Repository")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			Mvx.LazyConstructAndRegisterSingleton<IMediaPicker, MediaPicker>();
		}
	}
}
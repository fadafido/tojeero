using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using UIKit;
using Tojeero.Core;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.CrossCore.IoC;

namespace Tojeero.iOS
{
	public class Setup : MvxTouchSetup
	{
		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
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

		protected override IMvxTouchViewPresenter CreatePresenter()
		{
			return new MvxPagePresenter(Window);
		}

		protected override void InitializeFirstChance()
		{
			CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();
		}
	}
}
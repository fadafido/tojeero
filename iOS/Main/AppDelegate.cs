﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;

namespace Tojeero.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		UIWindow _window;

		public override bool FinishedLaunching(UIApplication app,
			NSDictionary options)
		{
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			var setup = new Setup(this, _window);
			setup.Initialize();

			global::Xamarin.Forms.Forms.Init();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			_window.MakeKeyAndVisible();


			return true;
		}
	}
}


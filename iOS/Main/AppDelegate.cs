using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Tojeero.Forms;
using Xamarin.Forms;
using Facebook.CoreKit;
using Parse;
using Tojeero.Core;
using ImageCircle.Forms.Plugin.iOS;
using Xamarin.Forms.Platform.iOS;


namespace Tojeero.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : MvxApplicationDelegate
	{
		#region Private API

		Xamarin.Forms.Application _current;
		UIWindow _window;
		private static AppDelegate _instance;
		#endregion

		#region Properties

		public static UIViewController RootViewController
		{
			get
			{
				return _instance._window != null ? _instance._window.RootViewController : null;
			}
		}

		#endregion

		#region Lifecycle Management

		public override bool FinishedLaunching(UIApplication app,
		                                       NSDictionary options)
		{
			_instance = this;

			_window = new UIWindow(UIScreen.MainScreen.Bounds);

			//Initialize Parse
			ParseInitialize.Initialize();

			//Initialize MvvmCross
			var setup = new Setup(this, _window);
			setup.Initialize();

			//Initialize Xamarin Forms
			global::Xamarin.Forms.Forms.Init();

			_current = new FormsApp();
			_window.RootViewController = _current.MainPage.CreateViewController();
			_window.MakeKeyAndVisible();

			//Initialize Misc Plugins
			ImageCircleRenderer.Init();

			MakeAppearanceCustomizations();

			//BootstrapData.GenerateSampleProductsAndStores();
			return ApplicationDelegate.SharedInstance.FinishedLaunching (app, options);
		}

		public override bool OpenUrl (UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		{
			// We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
			return ApplicationDelegate.SharedInstance.OpenUrl (application, url, sourceApplication, annotation);
		}
		#endregion

		#region Utility Methods

		private void MakeAppearanceCustomizations()
		{
			UINavigationBar.Appearance.BarTintColor = Colors.Blue.ToUIColor();
			UINavigationBar.Appearance.TintColor = Colors.Black.ToUIColor();
			UITextAttributes attr = new UITextAttributes();
			attr.Font = UIFont.FromName("System", 16);
			attr.TextColor = UIColor.White;
			UINavigationBar.Appearance.SetTitleTextAttributes(attr);
		}

		#endregion


	}
}


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
using Cirrious.MvvmCross.Platform;


namespace Tojeero.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : FormsApplicationDelegate, IMvxApplicationDelegate
	{
		#region Private API

		private static AppDelegate _instance;

		#endregion

		#region Lifecycle Management

		public override bool FinishedLaunching(UIApplication app,
		                                       NSDictionary options)
		{
			//Initialize Parse
			ParseInitialize.Initialize();

			//Initialize MvvmCross
			var setup = new Setup(this, null);
			setup.Initialize();

			//Initialize Xamarin Forms
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new FormsApp());

			//Initialize Misc Plugins
			ImageCircleRenderer.Init();

			MakeAppearanceCustomizations();
			base.FinishedLaunching(app, options);
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

		#region IMvxApplicationDelegate

		//
		// Methods
		//
		public override void DidEnterBackground(UIApplication application)
		{
			base.DidEnterBackground(application);
			this.FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
		}

		public override void FinishedLaunching(UIApplication application)
		{
			base.FinishedLaunching(application);
			this.FireLifetimeChanged(MvxLifetimeEvent.Launching);
		}

		private void FireLifetimeChanged(MvxLifetimeEvent which)
		{
			EventHandler<MvxLifetimeEventArgs> lifetimeChanged = this.LifetimeChanged;
			if (lifetimeChanged != null)
			{
				lifetimeChanged(this, new MvxLifetimeEventArgs(which));
			}
		}

		public override void WillEnterForeground(UIApplication application)
		{
			base.WillEnterForeground(application);
			this.FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
		}

		public override void WillTerminate(UIApplication application)
		{
			base.WillTerminate(application);
			this.FireLifetimeChanged(MvxLifetimeEvent.Closing);
		}

		//
		// Events
		//
		public event EventHandler<MvxLifetimeEventArgs> LifetimeChanged;

		#endregion
	}
}


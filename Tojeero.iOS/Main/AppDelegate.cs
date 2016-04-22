using System;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.Touch.Platform;
using Facebook.CoreKit;
using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Tojeero.Core;
using Tojeero.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Tojeero.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate, IMvxApplicationDelegate
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
            Xamarin.Forms.Forms.Init();

            LoadApplication(new FormsApp());

            //Initialize Misc Plugins
            ImageCircleRenderer.Init();
            MakeAppearanceCustomizations();
            base.FinishedLaunching(app, options);

            return ApplicationDelegate.SharedInstance.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        #endregion

        #region Utility Methods

        private void MakeAppearanceCustomizations()
        {
            UINavigationBar.Appearance.BarTintColor = Colors.Secondary.ToUIColor();
            UINavigationBar.Appearance.TintColor = UIColor.White;
            var attr = new UITextAttributes();
            attr.Font = UIFont.FromName("System", 16);
            attr.TextColor = UIColor.White;
            UINavigationBar.Appearance.SetTitleTextAttributes(attr);

            UITabBar.Appearance.SelectedImageTintColor = Colors.Secondary.ToUIColor();
        }

        #endregion

        #region IMvxApplicationDelegate

        //
        // Methods
        //
        public override void DidEnterBackground(UIApplication application)
        {
            base.DidEnterBackground(application);
            FireLifetimeChanged(MvxLifetimeEvent.Deactivated);
        }

        public override void FinishedLaunching(UIApplication application)
        {
            base.FinishedLaunching(application);
            FireLifetimeChanged(MvxLifetimeEvent.Launching);
        }

        private void FireLifetimeChanged(MvxLifetimeEvent which)
        {
            var lifetimeChanged = LifetimeChanged;
            if (lifetimeChanged != null)
            {
                lifetimeChanged(this, new MvxLifetimeEventArgs(which));
            }
        }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);
            FireLifetimeChanged(MvxLifetimeEvent.ActivatedFromMemory);
        }

        public override void WillTerminate(UIApplication application)
        {
            base.WillTerminate(application);
            FireLifetimeChanged(MvxLifetimeEvent.Closing);
        }

        //
        // Events
        //
        public event EventHandler<MvxLifetimeEventArgs> LifetimeChanged;

        #endregion
    }
}
using System;
using Android.App;
using System.Collections.Generic;
using Android.Content;
using Tojeero.Core;
using Xamarin;
using Xamarin.Facebook;
using ImageCircle.Forms.Plugin.Droid;
using Cirrious.CrossCore;

[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]

namespace Tojeero.Droid
{
	#if DEBUG
	[Application(Debuggable=true, Icon = "@drawable/icon")]
	#else
	[Application(Debuggable=false, Icon = "@drawable/icon")]
	#endif

	public class TojeeroApplication : Application
	{
		#region Private API

		ActivityLifecycleHandler _lifecycleHandler;

		#endregion

		#region Properties

		// A static instance of an application's singleton.
		public static TojeeroApplication Instance { get; private set; }

		#endregion

		#region Constructor

		public TojeeroApplication()
		{
			/* Default constructor */
		}

		protected TojeeroApplication(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) 
			: base(javaReference, transfer)
		{
			/* Nothing to do */
		}

		#endregion

		#region Lifecycle Management

		public override void OnCreate()
		{
			base.OnCreate();
			Instance = this;

			//Initialize Xamarin Insights
			string key = Constants.XamarinInsightsApiKey;
			#if DEBUG
			key = Insights.DebugModeKey;
			#endif
			Insights.Initialize(key, this);

			//Initialize Facebook
			FacebookSdk.SdkInitialize(this);

			//Initialize Parse
			ParseInitialize.Initialize();

			//Setup MvvmCross
			var setup = new Setup(this);
			setup.Initialize();

			//Initialize Misc Plugins
			ImageCircleRenderer.Init();
		}
			
		#endregion

		#region Private classes

		private class ActivityLifecycleHandler : Java.Lang.Object, IActivityLifecycleCallbacks
		{
			public Activity CurrentActivity { get; set; }
			#region IActivityLifecycleCallbacks implementation
			public void OnActivityCreated(Activity activity, Android.OS.Bundle savedInstanceState)
			{
				CurrentActivity = activity;
			}

			public void OnActivityDestroyed(Activity activity)
			{
				
			}
			public void OnActivityPaused(Activity activity)
			{
				
			}
			public void OnActivityResumed(Activity activity)
			{
				
			}
			public void OnActivitySaveInstanceState(Activity activity, Android.OS.Bundle outState)
			{
				
			}
			public void OnActivityStarted(Activity activity)
			{
				
			}
			public void OnActivityStopped(Activity activity)
			{
				
			}
			#endregion
			
		}

		#endregion
	}
}


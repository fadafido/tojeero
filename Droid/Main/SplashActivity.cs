using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Tojeero.Core;
using Xamarin;
using Xamarin.Facebook;
using ImageCircle.Forms.Plugin.Droid;
using System.Threading.Tasks;

namespace Tojeero.Droid
{
	[Activity(Label = "Tojeero", 
		Icon = "@drawable/icon", 
		Theme="@style/Theme.Splash", 
		MainLauncher = true,  
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
		ScreenOrientation=ScreenOrientation.Portrait,
		NoHistory = true)]
	[IntentFilter(new[] { Android.Content.Intent.ActionMain }, Categories = new[] { "android.intent.category.LAUNCHER" })]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

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
			StartActivity(typeof(MainActivity));
		}
	}
}


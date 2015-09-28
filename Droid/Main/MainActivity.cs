using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Tojeero.Forms;
using Xamarin.Facebook;
using Parse;
using Tojeero.Core;
using ImageCircle.Forms.Plugin.Droid;
using Android.Graphics.Drawables;
using Cirrious.CrossCore;
using XLabs.Platform.Services;

namespace Tojeero.Droid
{
	[Activity(Label = "Tojeero", Icon = "@drawable/icon", Theme="@style/Theme.Tojeero", MainLauncher = true,  ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	[IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { "android.intent.category.LAUNCHER" })]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		#region Properties

		public ICallbackManager CallbackManager { get; private set; }

		#endregion

		#region Lifecycle management

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			//Setup MvvmCross
			var setup = new Setup(this);
			setup.Initialize();

			//Setup Xamarin forms
			global::Xamarin.Forms.Forms.Init(this, bundle);
			LoadApplication(new FormsApp());

			//Initialize Facebook
			FacebookSdk.SdkInitialize(this);

			//Initialize Parse
			ParseClient.Initialize(Constants.ParseApplicationId, Constants.ParseDotNetKey);
			ParseFacebookUtils.Initialize(Constants.FacebookAppId);

			//Initialize Misc Plugins
			ImageCircleRenderer.Init();
			CallbackManager = CallbackManagerFactory.Create();

			makeUICustomizations();

		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}
			
		#endregion

		#region Utility Methods

		void makeUICustomizations()
		{
			ActionBar.SetIcon (new ColorDrawable(Resources.GetColor (Android.Resource.Color.Transparent)));
		}

		#endregion
	}
}


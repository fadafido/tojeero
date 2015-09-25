﻿using System;

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

namespace Tojeero.Droid
{
	[Activity(Label = "Test.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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

			CallbackManager = CallbackManagerFactory.Create();
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			CallbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		#endregion
	}
}


using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Java.Security;
using Tojeero.Forms;
using Xamarin.Facebook;
using Xamarin.Forms.Platform.Android;

namespace Tojeero.Droid
{
    [Activity(Label = "Tojeero",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Tojeero",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FormsApplicationActivity
    {
        #region Properties

        public ICallbackManager CallbackManager { get; private set; }

        #endregion

        #region Lifecycle management

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

#if DEBUG
            var info = PackageManager.GetPackageInfo("com.tojeero.tojeero", PackageInfoFlags.Signatures);

            foreach (var signature in info.Signatures)
            {
                var md = MessageDigest.GetInstance("SHA");
                md.Update(signature.ToByteArray());

                var keyhash = Convert.ToBase64String(md.Digest());
                Console.WriteLine("KeyHash:", keyhash);
            }
#endif
            //Setup Xamarin forms
            Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new FormsApp());

            CallbackManager = CallbackManagerFactory.Create();

            makeUICustomizations();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            CallbackManager.OnActivityResult(requestCode, (int) resultCode, data);
        }

        #endregion

        #region Utility Methods

        void makeUICustomizations()
        {
            ActionBar.SetIcon(new ColorDrawable(Resources.GetColor(Android.Resource.Color.Transparent)));
        }

        #endregion
    }
}
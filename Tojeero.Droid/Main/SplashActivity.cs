using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Tojeero.Droid
{
    [Activity(Label = "Tojeero",
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait,
        NoHistory = true)]
    [IntentFilter(new[] {Intent.ActionMain}, Categories = new[] {"android.intent.category.LAUNCHER"})]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            StartActivity(typeof (MainActivity));
        }
    }
}
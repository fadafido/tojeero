using System.Diagnostics;
using System.Reflection;
using Tojeero.Core;
using Tojeero.Forms;
using UIKit;
using Xamarin;

namespace Tojeero.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            var key = Constants.XamarinInsightsApiKey;
#if DEBUG
            key = Insights.DebugModeKey;

            //Log Resources

            var assembly = typeof (FormsApp).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                Debug.WriteLine("FOUND RESOURCE: " + res);

#endif
            Insights.Initialize(key);

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
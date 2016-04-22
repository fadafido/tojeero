using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin;
using Tojeero.Core;
using System.Reflection;

namespace Tojeero.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{	
			string key = Constants.XamarinInsightsApiKey;
			#if DEBUG
			key = Insights.DebugModeKey;

			//Log Resources

			var assembly = typeof(Tojeero.Forms.FormsApp).GetTypeInfo().Assembly;
			foreach (var res in assembly.GetManifestResourceNames())
				System.Diagnostics.Debug.WriteLine("FOUND RESOURCE: " + res);
			
			#endif
			Insights.Initialize(key);

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

	}
}


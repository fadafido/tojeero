using System;

using Xamarin.Forms;
using Cirrious.CrossCore;

namespace Tojeero.Forms
{
	public class FormsApp : Application
	{
		static FormsApp()
		{
			Mvx.Trace("aaaaaa");
		}

		public FormsApp()
		{
			// The root page of your application
			MainPage = new RootPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}


using System;

using Xamarin.Forms;
using Cirrious.CrossCore;
using Xamarin;
using Tojeero.Core;
using Tojeero.Core.Services;
using System.Reflection;
using Tojeero.Core.Resources;

namespace Tojeero.Forms
{
	public class FormsApp : Application
	{
		#region Private fields and properties

		private NavigationPage rootNavigation;

		#endregion

		#region Constructors

		public FormsApp()
		{
			// The root page of your application
			var bootstrap = new BootstrapPage() { Title = "Tojeero" };
			rootNavigation = new NavigationPage(bootstrap);
			bootstrap.ViewModel.BootstrapFinished = () =>
			{
				this.MainPage = new RootPage();
			};
			MainPage = rootNavigation;
		}

		#endregion

		protected override void OnStart()
		{
			// Handle when your app starts
			updateCulture();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
			updateCulture();
		}

		private void updateCulture()
		{
			if (Device.OS != TargetPlatform.WinPhone)
			{
				var localization = Mvx.Resolve<ILocalizationService>();
				AppResources.Culture = localization.Culture;
			}
		}
	}
}


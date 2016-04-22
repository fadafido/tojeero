using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Cirrious.CrossCore;
using Tojeero.Core.Resources;
using Tojeero.Core.Services;
using Tojeero.Core.Services.Contracts;
using Tojeero.Forms.Views.Main;

namespace Tojeero.Forms
{
	public partial class FormsApp : Application
	{
		#region Private fields and properties

		private NavigationPage rootNavigation;

		#endregion

		#region Constructors

		public FormsApp()
		{
			InitializeComponent();
			// The root page of your application
			var bootstrap = new BootstrapPage() { Title = AppResources.AppName };
			bootstrap.ViewModel.BootstrapFinished = () =>
				{
					this.MainPage = new RootPage();
				};
			MainPage = bootstrap;
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


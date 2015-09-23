using System;
using Android.App;
using Android.Content.PM;
using Xamarin.Forms.Platform.Android;
using Android.OS;
using Tojeero.Droid;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;
using Xamarin.Forms;
using Tojeero.Core;

namespace Tojeero.Droid
{
	[Activity(Label = "XFormsCross Template",
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MvxNavigationActivity
		: FormsApplicationActivity, IMvxPageNavigationProvider
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Xamarin.Forms.Forms.Init(this, bundle);

			var uiContext = new UIContext
				{
					CurrentContext = this
				};

			Mvx.Resolve<IMvxPageNavigationHost>().NavigationProvider = this;
			var start = Mvx.Resolve<IMvxAppStart>();
			start.Start();
		}

		public async void Push(Page page)
		{
			if (MvxNavigationActivity.NavigationPage != null)
			{
				await MvxNavigationActivity.NavigationPage.PushAsync(page);
				return;
			}

			MvxNavigationActivity.NavigationPage = new NavigationPage(page);
			this.SetPage(MvxNavigationActivity.NavigationPage);
		}

		public async void Pop()
		{
			if (MvxNavigationActivity.NavigationPage == null)
				return;

			await MvxNavigationActivity.NavigationPage.PopAsync();
		}

		public static NavigationPage NavigationPage { get; set; }
	}
}


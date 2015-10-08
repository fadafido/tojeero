using System;

using Xamarin.Forms;
using Tojeero.Core.Services;
using Cirrious.CrossCore;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		public RootPage()
		{						
			this.Master = new SideMenuPage()
			{
				Title = AppResources.AppName
			};
			var tabs = new TabbedPage(){ Title = AppResources.AppName };
			tabs.Children.Add(new ProductsPage() { Title = AppResources.TitleProducts });
			tabs.Children.Add(new StoresPage() { Title = AppResources.TitleStores });
			this.Detail = new NavigationPage(tabs);
		}
	}
}



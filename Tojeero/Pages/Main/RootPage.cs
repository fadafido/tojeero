using System;

using Xamarin.Forms;
using Tojeero.Core.Services;
using Cirrious.CrossCore;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		public RootPage()
		{			
			Mvx.Resolve<IAuthenticationService>().RestoreSavedSession();
			this.Master = new SideMenuPage()
			{
				Title = "Tojeero",
			};
			var tabs = new TabbedPage(){ Title = "Tojeero" };
			tabs.Children.Add(new ProductsPage() { Title = "Products" });
			tabs.Children.Add(new StoresPage() { Title = "Store" });
			this.Detail = new NavigationPage(tabs);
		}
	}
}



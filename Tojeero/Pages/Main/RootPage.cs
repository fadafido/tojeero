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
			this.Detail = new NavigationPage(new HomePage() {Title = "Tojeero"});
		}
	}
}



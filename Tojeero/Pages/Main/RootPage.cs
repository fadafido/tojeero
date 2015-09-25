using System;

using Xamarin.Forms;

namespace Tojeero.Forms
{
	public class RootPage : MasterDetailPage
	{
		public RootPage()
		{
			this.Master = new SideMenuPage()
			{
				Title = "Tojeero"
			};
			this.Detail = new NavigationPage(new HomePage());
		}
	}
}



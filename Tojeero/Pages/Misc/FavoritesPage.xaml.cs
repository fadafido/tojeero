using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Forms.Resources;

namespace Tojeero.Forms
{
	public partial class FavoritesPage : ContentPage
	{
		public FavoritesPage()
		{			
			InitializeComponent();
			this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonClose, "", async () =>
				{
					await this.Navigation.PopModalAsync();
				}));
		}
	}
}


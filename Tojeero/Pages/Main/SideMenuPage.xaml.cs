using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Core;

namespace Tojeero.Forms
{
	public partial class SideMenuPage : ContentPage
	{
		#region Properties

		private SideMenuViewModel _viewModel;
		public SideMenuViewModel ViewModel
		{
			get
			{
				return _viewModel;
			}
			set
			{
				if (_viewModel != value)
				{
					_viewModel = value;
					this.BindingContext = _viewModel;
				}
			}
		}

		#endregion

		#region Constructors

		public SideMenuPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<SideMenuViewModel>();
			this.ViewModel.ShowProfileSettings += showProfileSettings;
			InitializeComponent();
		}

		#endregion

		#region Utility Methods

		private async void showProfileSettings(object sender, EventArgs<bool> e)
		{
			await this.Navigation.PushModalAsync(new NavigationPage(new ProfileSettingsPage(e.Data)));
		}

		#endregion
	}
}


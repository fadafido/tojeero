using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Core;
using Tojeero.Forms.Resources;

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
					setupViewModel();
				}
			}
		}

		#endregion

		#region Constructors

		public SideMenuPage()
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<SideMenuViewModel>();
			InitializeComponent();
			this.Icon = "menuIcon.png";
		}

		#endregion

		#region View lifecycle management

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.LoadUserStoreCommand.Execute(null);
		}

		#endregion

		#region Utility methods

		private void setupViewModel()
		{			
			this.ViewModel.ShowProfileSettings = async (arg) => {
				await this.Navigation.PushModalAsync(new NavigationPage(new ProfileSettingsPage(arg)));
			};
			this.ViewModel.ShowLanguageChangeWarning = (arg) => {
				DisplayAlert(AppResources.AlertTitleWarning, arg, AppResources.OK);
			};

			this.ViewModel.ShowSaveStoreAction = async (s) => {
				if(s == null)
				{
					await this.Navigation.PushModalAsync(new NavigationPage(new SaveStorePage(s)));
				}
				else
				{
					var storeInfoPage = new StoreInfoPage(s, ContentMode.Edit);
					await this.Navigation.PushModalAsync(new NavigationPage(storeInfoPage));
				}
			};
			this.BindingContext = _viewModel;
		}

		#endregion
	}
}


using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;

namespace Tojeero.Forms
{
	public partial class ProfileSettingsPage : ContentPage
	{
		#region Properties

		private ProfileSettingsViewModel _viewModel;
		public ProfileSettingsViewModel ViewModel
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

		public ProfileSettingsPage(bool userShouldProvideDetails = false)
			: base()
		{
			this.ViewModel = MvxToolbox.LoadViewModel<ProfileSettingsViewModel>(new {userShouldProvideProfileDetails =  userShouldProvideDetails});
			this.ViewModel.Close += closeProfileSettings;
			InitializeComponent();
		}

		#endregion

		#region View Lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

		#endregion

		#region Utility Methods

		private async void closeProfileSettings(object sender, EventArgs e)
		{
			await this.Navigation.PopModalAsync();
		}

		#endregion
	}
}


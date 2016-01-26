using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tojeero.Core;
using Xamarin.Forms;
using Tojeero.Core.ViewModels;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Resources;

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
			this.ViewModel = MvxToolbox.LoadViewModel<ProfileSettingsViewModel>(new {userShouldProvideProfileDetails = userShouldProvideDetails});
			this.ViewModel.CloseAction = async () =>
			{
				await this.Navigation.PopModalAsync();
			};
			this.ViewModel.ShowTermsAction = async () =>
			{
				await this.Navigation.PushModalAsync(new NavigationPage(new TermsPage()));
			};
			InitializeComponent();
			if (!this.ViewModel.UserShouldProvideProfileDetails)
			{
				this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonDone, null, () =>
						{
							this.ViewModel.SubmitCommand.Execute(null);
						}));
				this.ToolbarItems.Add(new ToolbarItem(AppResources.ButtonCancel, null, async () =>
						{
							await this.Navigation.PopModalAsync();
						}, priority: 15));
			}
		    setupPickers();
		}

		#endregion

		#region View Lifecycle

		protected override void OnAppearing()
		{
			base.OnAppearing();
			this.ViewModel.ReloadCommand.Execute(null);
		}

        #endregion

        #region Utility methods

        private void setupPickers()
        {
            countriesPicker.FacetsLoader = this.ViewModel.FetchCountryFacets;
            countriesPicker.ObjectsLoader = () => Task<IList<ICountry>>.Factory.StartNew(() => this.ViewModel.Countries);

            citiesPicker.FacetsLoader = this.ViewModel.FetchCityFacets;
            citiesPicker.ObjectsLoader = () => Task<IList<ICity>>.Factory.StartNew(() => this.ViewModel.Country?.Cities);
        }

        #endregion
    }
}


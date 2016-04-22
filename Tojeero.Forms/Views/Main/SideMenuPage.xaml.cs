using Tojeero.Core.Model;
using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Main;
using Tojeero.Forms.Views.Store;
using Tojeero.Forms.Views.User;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Main
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

			this.ViewModel.ShowTermsAction = async () =>
			{
					await this.Navigation.PushModalAsync(new NavigationPage(new TermsPage()));
			};
			this.BindingContext = _viewModel;
		}

		#endregion
	}
}


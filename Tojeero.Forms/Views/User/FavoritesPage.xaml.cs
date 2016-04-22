using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.User;
using Tojeero.Forms.Views.Product;
using Tojeero.Forms.Views.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.User
{
    public partial class FavoritesPage : ContentPage
    {
        #region Constructors

        public FavoritesPage()
        {
            ViewModel = MvxToolbox.LoadViewModel<FavoritesViewModel>();
            InitializeComponent();
            NavigationPage.SetTitleIcon(this, "tojeero.png");
        }

        #endregion

        #region View lifecycle management

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadFavoriteCountsCommand.Execute(null);
        }

        #endregion

        #region Properties

        private FavoritesViewModel _viewModel;

        public FavoritesViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                setupViewModel();
            }
        }

        #endregion

        #region Utility methods

        private void setupViewModel()
        {
            ViewModel.ShowFavoriteProductsAction =
                async () => { await Navigation.PushAsync(new FavoriteProductsPage()); };
            ViewModel.ShowFavoriteStoresAction = async () => { Navigation.PushAsync(new FavoriteStoresPage()); };
            BindingContext = ViewModel;
        }

        #endregion
    }
}
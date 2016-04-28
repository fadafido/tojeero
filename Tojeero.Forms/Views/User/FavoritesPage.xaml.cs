using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.User;
using Tojeero.Forms.Views.Product;
using Tojeero.Forms.Views.Store;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.User
{
    public partial class FavoritesPage
    {
        #region Constructors

        public FavoritesPage()
        {
            InitializeComponent();

            ViewModel = MvxToolbox.LoadViewModel<FavoritesViewModel>();
            
            NavigationPage.SetTitleIcon(this, "tojeero.png");
        }

        #endregion

        #region Parent override

        protected override void SetupViewModel()
        {
            base.SetupViewModel();
            ViewModel.ShowFavoriteProductsAction =
                async () => { await Navigation.PushAsync(new FavoriteProductsPage()); };
            ViewModel.ShowFavoriteStoresAction = async () =>
            {
                await Navigation.PushAsync(new FavoriteStoresPage());
            };
        }

        #endregion

    }
}
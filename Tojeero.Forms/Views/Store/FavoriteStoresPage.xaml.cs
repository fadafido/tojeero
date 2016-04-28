using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class FavoriteStoresPage
    {
        #region Constructors

        public FavoriteStoresPage()
        {
            InitializeComponent();
            var vm = MvxToolbox.LoadViewModel<FavoriteStoresViewModel>();
            vm.ShowStoreInfoAction = async s =>
            {
                var storeInfoPage = new StoreInfoPage(s);
                await Navigation.PushAsync(storeInfoPage);
            };
            ViewModel = vm;
        }

        #endregion
    }
}
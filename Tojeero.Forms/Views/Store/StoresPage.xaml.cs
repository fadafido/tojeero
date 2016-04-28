using Tojeero.Core.Resources;
using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Store
{
    public partial class StoresPage : BaseSearchableTabPage
    {
        #region Constructors

        public StoresPage()
        {
            InitializeComponent();

            ProductsButton.IsSelected = false;
            StoresButton.IsSelected = true;

            var vm = MvxToolbox.LoadViewModel<StoresViewModel>();
            vm.ShowStoreInfoAction = async s =>
            {
                var storeInfo = new StoreInfoPage(s);
                await Navigation.PushAsync(storeInfo);
            };

            ViewModel = vm;

            ToolbarItems.Add(new ToolbarItem("", "filterIcon.png",
                async () =>
                {
                    await Navigation.PushModalAsync(new NavigationPage(new FilterStoresPage(ViewModel.SearchQuery)));
                }));
            SearchBar.Placeholder = AppResources.PlaceholderSearchStores;

        }

        #endregion
    }
}
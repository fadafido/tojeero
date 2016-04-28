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
        #region Properties

        public new StoresViewModel ViewModel
        {
            get { return base.ViewModel as StoresViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Constructors

        public StoresPage()
        {
            InitializeComponent();

            ProductsButton.IsSelected = false;
            StoresButton.IsSelected = true;

            ViewModel = MvxToolbox.LoadViewModel<StoresViewModel>();
            ToolbarItems.Add(new ToolbarItem("", "filterIcon.png",
                async () =>
                {
                    await Navigation.PushModalAsync(new NavigationPage(new FilterStoresPage(ViewModel.SearchQuery)));
                }));
            SearchBar.Placeholder = AppResources.PlaceholderSearchStores;
            ListViewEx.ItemSelected += itemSelected;
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.ViewModel.LoadFirstPageCommand.Execute(null);
        }

        #endregion

        #region UI Events

        private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = ((ListViewEx) sender).SelectedItem as StoreViewModel;
            if (item != null)
            {
                ((ListViewEx) sender).SelectedItem = null;
                var storeInfo = new StoreInfoPage(item.Store);
                await Navigation.PushAsync(storeInfo);
            }
        }

        #endregion
    }
}
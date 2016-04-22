using Tojeero.Core.Toolbox;
using Tojeero.Core.ViewModels.Store;
using Tojeero.Forms.Views.Common;
using Xamarin.Forms;
using ListView = Tojeero.Forms.Controls.ListView;

namespace Tojeero.Forms.Views.Store
{
    public partial class FavoriteStoresPage : BaseCollectionPage
    {
        #region Constructors

        public FavoriteStoresPage()
        {
            InitializeComponent();
            ViewModel = MvxToolbox.LoadViewModel<FavoriteStoresViewModel>();
            ListView.ItemSelected += itemSelected;
        }

        #endregion

        #region Properties

        public new FavoriteStoresViewModel ViewModel
        {
            get { return base.ViewModel as FavoriteStoresViewModel; }
            set { base.ViewModel = value; }
        }

        #endregion

        #region Page lifecycle

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.LoadFirstPageCommand.Execute(null);
        }

        #endregion

        #region UI Events

        private async void itemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = ((ListView) sender).SelectedItem as StoreViewModel;
            if (item != null)
            {
                ((ListView) sender).SelectedItem = null;
            }
        }

        #endregion
    }
}
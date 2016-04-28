using System;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Views.Main;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseSearchableTabPage
    {
        #region Properties

        public ISearchViewModel ViewModel
        {
            get { return base.ViewModel; }
            set
            {
                if (ViewModel != value)
                {
                    DisconnectEvents();
                    base.ViewModel = value;
                    ConnectEvents();
                }
            }
        }

        #endregion

        #region Constructors

        public BaseSearchableTabPage()
        {
            InitializeComponent();
            NavigationPage.SetTitleIcon(this, "tojeero.png");
        }

        #endregion

        #region Properties

        public DataTemplate ItemTemplate
        {
            get { return ListViewEx.ItemTemplate; }
            set { ListViewEx.ItemTemplate = value; }
        }

        public ListViewEx ListViewEx
        {
            get { return _listViewEx; }
        }

        public SearchBar SearchBar
        {
            get { return searchBar; }
        }

        public TabButton ProductsButton
        {
            get { return productsTabButton; }
        }

        public TabButton StoresButton
        {
            get { return storesTabButton; }
        }

        #endregion

        #region Protected API

        protected virtual void ConnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished += ReloadFinished;
            }
        }

        protected virtual void DisconnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished -= ReloadFinished;
            }
        }

        #endregion

        #region Event Handlers

        void ReloadFinished(object sender, EventArgs e)
        {
            ListViewEx.EndRefresh();
        }

        protected void ProductButtonClicked(object sender, EventArgs e)
        {
            var root = this.FindParent<RootPage>();
            if (root != null)
            {
                root.SelectProductsPage();
            }
        }

        protected void StoreButtonClicked(object sender, EventArgs e)
        {
            var root = this.FindParent<RootPage>();
            if (root != null)
            {
                root.SelectStoresPage();
            }
        }

        #endregion
    }
}
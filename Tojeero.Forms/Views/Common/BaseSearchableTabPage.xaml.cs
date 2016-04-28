using System;
using Tojeero.Core.ViewModels.Contracts;
using Tojeero.Forms.Controls;
using Tojeero.Forms.Toolbox;
using Tojeero.Forms.Views.Main;
using Xamarin.Forms;

namespace Tojeero.Forms.Views.Common
{
    public partial class BaseSearchableTabPage : ContentPage
    {
        #region Properties

        private ISearchViewModel _viewModel;

        public ISearchViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    DisconnectEvents();
                    _viewModel = value;
                    ConnectEvents();
                    BindingContext = _viewModel;
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
                ViewModel.ReloadFinished += reloadFinished;
            }
        }

        protected virtual void DisconnectEvents()
        {
            if (ViewModel != null)
            {
                ViewModel.ReloadFinished -= reloadFinished;
            }
        }

        #endregion

        #region Event Handlers

        void reloadFinished(object sender, EventArgs e)
        {
            ListViewEx.EndRefresh();
        }

        protected void productButtonClicked(object sender, EventArgs e)
        {
            var root = this.FindParent<RootPage>();
            if (root != null)
            {
                root.SelectProductsPage();
            }
        }

        protected void storeButtonClicked(object sender, EventArgs e)
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